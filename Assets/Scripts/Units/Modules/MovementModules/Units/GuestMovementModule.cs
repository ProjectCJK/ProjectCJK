using System;
using System.Collections.Generic;
using Interfaces;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Modules.MovementModules.Units
{
    public interface IGuestMovementModule : IInitializable<Vector3, List<Transform>>
    {
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
        private bool _isPathReady; // 경로가 계산되었는지 여부
        private float _movementSpeed => _guestStatModule.MovementSpeed;

        private List<Transform> _destinations;
        private int index;

        public GuestMovementModule(Guest guest, IGuestStatModule guestStatModule)
        {
            _guestStatModule = guestStatModule;
            _guestTransform = guest.transform;

            _navMeshAgent = guest.GetComponent<NavMeshAgent>();

            // NavMeshAgent 설정 추가
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.autoTraverseOffMeshLink = false; // 자동 오프메시 링크 이동 비활성화
            _navMeshAgent.autoBraking = false; // 목적지 도착 시 자동 정지 비활성화
            _navMeshAgent.stoppingDistance = 0.1f; // 정지 거리를 작게 설정
        }

        public void Initialize(Vector3 startPosition, List<Transform> destinations)
        {
            _destinations = destinations;
            index = 0;
            
            _navMeshAgent.isStopped = true;
            _navMeshAgent.speed = _movementSpeed;
            
            if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _guestTransform.position = hit.position;
                _navMeshAgent.isStopped = false;
            }

            if (_navMeshAgent.isOnNavMesh)
            {
                SetDestination(_destinations[index].position);   
            }
        }

        private void SetDestination(Vector3 destination)
        {
            _navMeshAgent.ResetPath();
            
            var path = new NavMeshPath();
            
            if (NavMesh.CalculatePath(_guestTransform.position, destination, NavMesh.AllAreas, path))
            {
                _navMeshAgent.SetPath(path);
            }
        }

        public void FixedUpdate()
        {
            if (_navMeshAgent != null && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                // 현재 위치에서 목적지까지 경로 확인
                var path = new NavMeshPath();
                
                if (NavMesh.CalculatePath(_guestTransform.position, _destinations[index].position, NavMesh.AllAreas, path))
                {
                    _navMeshAgent.SetPath(path);
                }
            }
        }
    }
}