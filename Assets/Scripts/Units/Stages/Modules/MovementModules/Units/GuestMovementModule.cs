using Interfaces;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Modules.MovementModules.Units
{
    public interface IGuestMovementModule : IInitializable<Vector3>
    {
        void Update();
        void FixedUpdate();
        void SetDestination(Vector3 destination);
        void ActivateNavMeshAgent(bool value);
    }

    public class GuestMovementModule : MovementModuleWithNavMeshAgent, IGuestMovementModule
    {
        private readonly CreatureStateMachine _creatureStateMachine;
        private readonly IGuestStatModule _guestStatModule;
        private readonly Transform _guestTransform;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _spriteTransform;

        private Vector3 _destination;
        private bool _isFacingRight;
        private bool _isMoving;

        public GuestMovementModule(
            Guest guest,
            IGuestStatModule guestStatModule,
            CreatureStateMachine creatureStateMachine,
            Transform spriteTransform)
        {
            _guestStatModule = guestStatModule;
            _guestTransform = guest.transform;
            _spriteTransform = spriteTransform;
            _creatureStateMachine = creatureStateMachine;
            _navMeshAgent = guest.GetComponent<NavMeshAgent>();

            // NavMeshAgent 설정 추가
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.autoTraverseOffMeshLink = false; // 자동 오프메시 링크 이동 비활성화
            _navMeshAgent.autoBraking = false; // 목적지 도착 시 자동 정지 비활성화
            _navMeshAgent.stoppingDistance = 1f; // 정지 거리를 작게 설정
            _navMeshAgent.acceleration = 120f;
        }

        private float _movementSpeed => _guestStatModule.MovementSpeed;

        public void Initialize(Vector3 startPosition)
        {
            ActivateNavMeshAgent(false);
            
            _navMeshAgent.speed = _movementSpeed;

            if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                _guestTransform.position = hit.position;
                Debug.Log($"Guest initialized at: {hit.position}, StartPosition: {startPosition}, Difference: {Vector3.Distance(hit.position, startPosition)}");
            }
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            if (TryCalculateAndSetPath(destination))
            {
                ActivateNavMeshAgent(true);
            }
        }

        public void Update()
        {
            if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
            {
                ActivateNavMeshAgent(true);
            }
            
            // 이동 상태가 변하지 않았으면 로직을 건너뛰기
            if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude < 0.01f)
            {
                HandleStateUpdate(); // 정지 상태 업데이트
                return;
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
            if (_navMeshAgent.enabled && !_navMeshAgent.isStopped && _navMeshAgent.isOnNavMesh)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
                {
                    TryCalculateAndSetPath(_destination);
                }
            }
        }

        public void ActivateNavMeshAgent(bool value)
        {
            _navMeshAgent.isStopped = !value;
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
        
        private bool TryCalculateAndSetPath(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(_guestTransform.position, hit.position, NavMesh.AllAreas, path))
                {
                    _navMeshAgent.SetPath(path);
                    return true;
                }
            }
            return false;
        }

    }
}