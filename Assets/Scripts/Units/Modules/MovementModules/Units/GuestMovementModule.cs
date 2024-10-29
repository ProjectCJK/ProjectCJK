using Interfaces;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Modules.MovementModules.Units
{
    public interface IGuestMovementModule : IInitializable<Vector3>
    {
        public void Update();
        public void FixedUpdate();
    }

    public class GuestMovementModule : MovementModule, IGuestMovementModule
    {
        private readonly IGuestStatModule _guestStatModule;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _guestTransform;

        private Vector3 _destination;
        private bool _isInitialized;
        private bool _hasReachedDestination;
        private float _movementSpeed => _guestStatModule.MovementSpeed;

        public GuestMovementModule(Guest guest, IGuestStatModule guestStatModule)
        {
            _guestStatModule = guestStatModule;
            _navMeshAgent = guest.GetComponent<NavMeshAgent>();
            _guestTransform = guest.transform;

            // NavMeshAgent의 업데이트를 중지하여 수동 제어
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.speed = _movementSpeed;
            _navMeshAgent.stoppingDistance = 1f;
        }

        public void Initialize(Vector3 destination)
        {
            SetDestination(destination);
        }

        private void SetDestination(Vector3 destination)
        {
            // 목적지를 NavMesh.SamplePosition을 사용해 설정
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                _destination = hit.position;
                _isInitialized = true;
                _hasReachedDestination = false;
                _navMeshAgent.isStopped = false; // 이동을 다시 활성화
                _navMeshAgent.SetDestination(_destination);
                Debug.Log($"Valid destination found: {_destination}");
            }
            else
            {
                _isInitialized = false;
                Debug.LogWarning("No valid destination found within the given radius.");
            }
        }

        public void Update()
        {
            if (!_isInitialized) return;

            // NavMeshAgent의 상태 출력 (디버그용)
            Debug.Log($"Current Position: {_guestTransform.position}, Destination: {_destination}");
            Debug.Log($"Remaining Distance: {_navMeshAgent.remainingDistance}, Stopping Distance: {_navMeshAgent.stoppingDistance}");
            Debug.Log($"Desired Velocity: {_navMeshAgent.desiredVelocity}");
            Debug.Log($"Path Status: {_navMeshAgent.pathStatus}");
        }

        public void FixedUpdate()
        {
            if (!_isInitialized || _navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete) return;

            // 목표 지점에 가까워지면 이동 중지
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_hasReachedDestination)
                {
                    // 목적지에 도달하면 NavMeshAgent를 멈춤
                    _navMeshAgent.isStopped = true;
                    _hasReachedDestination = true;
                    Debug.Log("Reached destination. Waiting for new destination...");
                }

                return;
            }

            // NavMeshAgent의 회피 정보를 고려한 방향으로 이동
            Vector2 movementDirection = GetMovementDirectionUsingAgent();
            MoveWithCollision(movementDirection);

            // NavMeshAgent의 위치를 업데이트하여 경로를 유지
            _navMeshAgent.nextPosition = _guestTransform.position;
        }

        private Vector2 GetMovementDirectionUsingAgent()
        {
            // NavMeshAgent의 desiredVelocity를 사용하여 이동 방향 계산
            Vector2 direction = _navMeshAgent.desiredVelocity.normalized;
            return direction * (_movementSpeed * Time.fixedDeltaTime);
        }

        private void MoveWithCollision(Vector2 move)
        {
            // 충돌 감지 후 이동
            if (!IsColliding(move))
            {
                _guestTransform.position += (Vector3)move;
            }
        }

        private bool IsColliding(Vector3 move)
        {
            // 충돌 감지 로직
            Vector3 currentPosition = _guestTransform.position;
            RaycastHit2D hit = Physics2D.CircleCast(currentPosition, _navMeshAgent.radius, move.normalized, move.magnitude, collisionLayerMask);

            return hit.collider != null;
        }
    }
}