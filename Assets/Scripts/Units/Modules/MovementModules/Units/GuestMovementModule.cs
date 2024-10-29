using System;
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
    }

    public class GuestMovementModule : MovementModule, IGuestMovementModule
    {
        private readonly IGuestStatModule _guestStatModule;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _guestTransform;

        private Vector3 _destination;
        private bool _isInitialized;
        private bool _hasReachedDestination;
        private bool _isPathReady; // 경로가 계산되었는지 여부
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
            _navMeshAgent.stoppingDistance = 2f;
        }

        public void Initialize(Vector3 destination)
        {
            SetDestination(destination);
        }

        public void SetDestination(Vector3 destination)
        {
            // 목적지를 NavMesh.SamplePosition을 사용해 설정
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                _destination = hit.position;
                _isInitialized = true;
                _isPathReady = false;
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

            // 경로가 계산될 때까지 대기
            if (!_isPathReady && !_navMeshAgent.pathPending)
            {
                _isPathReady = _navMeshAgent.hasPath;
                if (_isPathReady)
                {
                    Debug.Log("Path calculation completed. Ready for movement.");
                }
            }

            // 경로가 준비되지 않았거나 계산이 완료되지 않았다면 이동하지 않음
            if (!_isPathReady || _navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete) return;

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

            // NavMeshAgent의 velocity를 사용하여 이동 처리
            MoveUsingAgentVelocity();
        }

        private void MoveUsingAgentVelocity()
        {
            // NavMeshAgent의 velocity를 사용하여 이동 방향 계산
            Vector3 velocity = _navMeshAgent.velocity * Time.deltaTime;

            // 이동을 적용
            if (velocity != Vector3.zero)
            {
                _guestTransform.position += velocity;
            }

            // NavMeshAgent의 위치를 업데이트하여 경로를 유지
            _navMeshAgent.nextPosition = _guestTransform.position;
        }
    }
}