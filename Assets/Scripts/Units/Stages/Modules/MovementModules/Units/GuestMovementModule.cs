using System;
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
        void SetDestination(Tuple<string, Transform, Action> destination);
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
        private Action _onArrived;

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

            // 초기 위치를 NavMesh 위로 조정
            if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                _guestTransform.position = hit.position;
                Debug.Log($"Guest initialized at: {hit.position}");
            }
            else
            {
                Debug.LogWarning($"Failed to initialize guest at position: {startPosition}");
            }
        }

        public void SetDestination(Tuple<string, Transform, Action> destination)
        {
            _destination = destination.Item2.position;
            _onArrived = destination.Item3;

            // 경로 계산 및 설정 시도
            if (TryCalculateAndSetPath(_destination))
            {
                ActivateNavMeshAgent(true);
            }
            else
            {
                Debug.LogWarning($"Failed to calculate path to destination: {_destination}");
            }
        }

        public void Update()
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && !_navMeshAgent.pathPending)
            {
                ActivateNavMeshAgent(false);
                _onArrived?.Invoke();
                _onArrived = null;
            }

            // 이동 상태 업데이트
            HandleStateUpdate();

            // 스프라이트 방향 업데이트
            UpdateSpriteDirection();
        }

        public void FixedUpdate()
        {
            if (_navMeshAgent.enabled && !_navMeshAgent.isStopped && _navMeshAgent.isOnNavMesh)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
                {
                    if (!TryCalculateAndSetPath(_destination))
                    {
                        Debug.LogWarning($"FixedUpdate: Failed to recalculate path to {_destination}");
                    }
                }
            }
        }

        public void ActivateNavMeshAgent(bool value)
        {
            _navMeshAgent.isStopped = !value;
        }

        private void UpdateSpriteDirection()
        {
            if (_navMeshAgent.velocity.x > 0 && !_isFacingRight)
            {
                FlipSprite(true);
            }
            else if (_navMeshAgent.velocity.x < 0 && _isFacingRight)
            {
                FlipSprite(false);
            }
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
            // NavMesh 샘플링
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(_guestTransform.position, hit.position, NavMesh.AllAreas, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    _navMeshAgent.SetPath(path);
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Path calculation failed or incomplete to: {hit.position}");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to sample position near: {position}");
            }

            // 대체 동작: 가까운 NavMesh 지점 설정
            return HandlePathFailure(position);
        }

        private bool HandlePathFailure(Vector3 originalPosition)
        {
            if (NavMesh.SamplePosition(originalPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas)) // 5m 반경으로 확장
            {
                Debug.Log($"Adjusted destination to nearest NavMesh point: {hit.position}");
                _navMeshAgent.SetDestination(hit.position);
                return true;
            }

            Debug.LogError($"Unable to recover path near: {originalPosition}");
            return false;
        }
    }
}