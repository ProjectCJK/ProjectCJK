using System;
using System.Collections;
using Interfaces;
using Managers;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Modules.MovementModules.Units
{
    public interface IHunterMovementModule : IInitializable<Vector3>
    {
        public Vector2 Direction { get; }

        void Update();
        void FixedUpdate();
        void SetDestination(Tuple<string, Transform> destination);
        void ActivateNavMeshAgent(bool value);
        void HandleOnHit();
    }

    public class HunterMovementModule : MovementModuleWithNavMeshAgent, IHunterMovementModule
    {
        private const float MonsterFollowDistance = 2f; // 몬스터를 따라갈 때 유지할 거리
        private readonly CreatureStateMachine _creatureStateMachine;

        private readonly IHunterStatsModule _hunterStatModule;
        private readonly Transform _hunterTransform;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _spriteTransform;

        private Tuple<string, Transform> _destinationTransform;
        private bool _isFacingRight;
        private bool _isMoving;
        private bool _isSlowed;
        private Coroutine _slowCoroutine;

        public HunterMovementModule(
            Hunter guest,
            IHunterStatsModule hunterStatsModule,
            CreatureStateMachine creatureStateMachine,
            Transform spriteTransform)
        {
            _hunterStatModule = hunterStatsModule;
            _hunterTransform = guest.transform;
            _spriteTransform = spriteTransform;
            _creatureStateMachine = creatureStateMachine;
            _navMeshAgent = guest.GetComponent<NavMeshAgent>();

            // NavMeshAgent 설정 추가
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.autoTraverseOffMeshLink = false;
            _navMeshAgent.autoBraking = false;
            _navMeshAgent.stoppingDistance = 0.1f;
            _navMeshAgent.acceleration = 120f;
        }

        private float _movementSpeed => _hunterStatModule.MovementSpeed;
        public Vector2 Direction { get; private set; }

        public void Initialize(Vector3 startPosition)
        {
            _navMeshAgent.speed = _movementSpeed;

            if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                _hunterTransform.position = hit.position;

            ActivateNavMeshAgent(false);
        }

        public void SetDestination(Tuple<string, Transform> destination)
        {
            _destinationTransform = destination;

            if (_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.ResetPath();

                // 몬스터 대상일 경우 일정 거리 유지 설정
                if (_destinationTransform.Item1 == $"{ECreatureType.Monster}" && _destinationTransform.Item2 != null)
                {
                    Vector3 directionToTarget =
                        (_destinationTransform.Item2.position - _hunterTransform.position).normalized;
                    Vector3 adjustedPosition =
                        _destinationTransform.Item2.position - directionToTarget * MonsterFollowDistance;
                    _navMeshAgent.SetDestination(adjustedPosition);
                }
                else
                {
                    _navMeshAgent.SetDestination(_destinationTransform.Item2.position);
                }

                ActivateNavMeshAgent(true);
            }
        }

        public void Update()
        {
            if (_destinationTransform != null &&
                Vector3.Distance(_hunterTransform.position, _destinationTransform.Item2.position) > 0.5f)
            {
                Vector3 direction3D = (_destinationTransform.Item2.position - _hunterTransform.position).normalized;
                Direction = new Vector2(direction3D.x, direction3D.y);
                SetDestination(_destinationTransform);
            }

            switch (_navMeshAgent.velocity.x)
            {
                case > 0 when !_isFacingRight:
                    FlipSprite(true);
                    break;
                case < 0 when _isFacingRight:
                    FlipSprite(false);
                    break;
            }

            HandleStateUpdate();
        }

        public void FixedUpdate()
        {
            if (_navMeshAgent != null && !_navMeshAgent.isStopped && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                var path = new NavMeshPath();

                Vector3 targetPosition = _destinationTransform.Item1 == $"{ECreatureType.Monster}" &&
                                         _destinationTransform.Item2 != null
                    ? _destinationTransform.Item2.position - (Vector3)Direction * MonsterFollowDistance
                    : _destinationTransform.Item2.position;

                if (NavMesh.CalculatePath(_hunterTransform.position, targetPosition, NavMesh.AllAreas, path))
                    _navMeshAgent.SetPath(path);
            }
        }

        public void ActivateNavMeshAgent(bool value)
        {
            _navMeshAgent.isStopped = !value;
        }

        public void HandleOnHit()
        {
            if (_slowCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_slowCoroutine);
            _slowCoroutine = CoroutineManager.Instance.StartCoroutine(SlowDownTemporarily());
        }

        private void FlipSprite(bool faceRight)
        {
            _isFacingRight = faceRight;
            Vector3 scale = _spriteTransform.localScale;
            scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _spriteTransform.localScale = scale;
        }

        private void HandleStateUpdate()
        {
            UpdateMovementFlag();
            UpdateStateMachine();
        }

        private void UpdateMovementFlag()
        {
            _isMoving = _navMeshAgent.velocity.sqrMagnitude > 0.1f;
        }

        private void UpdateStateMachine()
        {
            _creatureStateMachine.ChangeState(_isMoving
                ? _creatureStateMachine.CreatureRunState
                : _creatureStateMachine.CreatureIdleState);
        }

        private IEnumerator SlowDownTemporarily()
        {
            _isSlowed = true;
            yield return new WaitForSeconds(0.5f);
            _isSlowed = false;
        }
    }
}