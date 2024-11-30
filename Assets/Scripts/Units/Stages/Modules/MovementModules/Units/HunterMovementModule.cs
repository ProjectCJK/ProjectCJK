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
        private const float MinFollowDistance = 1f; // 최소 거리
        private const float MaxFollowDistance = 3f; // 최대 거리
        private readonly CreatureStateMachine _creatureStateMachine;

        private readonly IHunterStatsModule _hunterStatModule;
        private readonly Transform _hunterTransform;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _spriteTransform;

        private Tuple<string, Transform> _destinationTransform;
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

            // NavMeshAgent 설정
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.autoTraverseOffMeshLink = false; // 자동 오프메시 링크 이동 비활성화
            _navMeshAgent.autoBraking = false; // 목적지 도착 시 자동 정지 비활성화
            _navMeshAgent.stoppingDistance = MinFollowDistance;
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

            if (_navMeshAgent.isOnNavMesh && _destinationTransform.Item2 != null)
            {
                _navMeshAgent.ResetPath();

                // NavMeshPath 생성 및 계산
                NavMeshPath path = new NavMeshPath();
                bool pathCalculated = NavMesh.CalculatePath(_hunterTransform.position, _destinationTransform.Item2.position, NavMesh.AllAreas, path);

                if (pathCalculated && path.status == NavMeshPathStatus.PathComplete)
                {
                    // 경로 상의 거리 계산
                    float navMeshDistance = CalculatePathDistance(path);

                    if (navMeshDistance > MaxFollowDistance)
                    {
                        // 최대 거리 초과 시 경로를 따라 이동
                        _navMeshAgent.SetPath(path);
                        ActivateNavMeshAgent(true);
                    }
                    else if (navMeshDistance < MinFollowDistance)
                    {
                        // 최소 거리 미만이면 멈춤
                        ActivateNavMeshAgent(false);
                    }
                    else
                    {
                        // 유효한 거리 범위 내에서 대기
                        ActivateNavMeshAgent(false);
                    }
                }
                else
                {
                    // 경로 계산 실패 시 대체 행동
                    Debug.LogWarning($"Path to {_destinationTransform.Item2.position} is invalid. Recalculating...");

                    if (NavMesh.SamplePosition(_destinationTransform.Item2.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                    {
                        _navMeshAgent.SetDestination(hit.position);
                        ActivateNavMeshAgent(true);
                    }
                    else
                    {
                        Debug.LogError($"Failed to find a valid NavMesh position near {_destinationTransform.Item2.position}");
                        ActivateNavMeshAgent(false);
                    }
                }
            }
        }

        public void Update()
        {
            if (_destinationTransform != null && _destinationTransform.Item2 != null)
            {
                NavMeshPath path = new NavMeshPath();
                bool pathCalculated = NavMesh.CalculatePath(_hunterTransform.position, _destinationTransform.Item2.position, NavMesh.AllAreas, path);

                if (pathCalculated && path.status == NavMeshPathStatus.PathComplete)
                {
                    float navMeshDistance = CalculatePathDistance(path);

                    if (navMeshDistance > MinFollowDistance)
                    {
                        // 추적 방향 업데이트
                        Vector3 direction3D = (_destinationTransform.Item2.position - _hunterTransform.position).normalized;
                        Direction = new Vector2(direction3D.x, direction3D.y);

                        SetDestination(_destinationTransform);
                    }
                }
            }

            FlipSpriteBasedOnDirection();
            HandleStateUpdate();
        }

        public void FixedUpdate()
        {
            if (_navMeshAgent != null && !_navMeshAgent.isStopped && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                var path = new NavMeshPath();

                Vector3 targetPosition = _destinationTransform.Item2.position;

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

        private void FlipSpriteBasedOnDirection()
        {
            // Direction.x를 기준으로 스프라이트 플립
            if ((Direction.x > 0 && _spriteTransform.localScale.x < 0) ||
                (Direction.x < 0 && _spriteTransform.localScale.x > 0))
            {
                Vector3 scale = _spriteTransform.localScale;
                scale.x = -scale.x;
                _spriteTransform.localScale = scale;
            }
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
        
        /// <summary>
        /// NavMesh 경로를 기준으로 거리를 계산합니다.
        /// </summary>
        private float CalculatePathDistance(NavMeshPath path)
        {
            float totalDistance = 0f;

            for (int i = 1; i < path.corners.Length; i++)
            {
                totalDistance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return totalDistance;
        }
    }
}