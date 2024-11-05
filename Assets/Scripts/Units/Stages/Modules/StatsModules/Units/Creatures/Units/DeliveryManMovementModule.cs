using Interfaces;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Modules.StatsModules.Units.Creatures.Units
{
    public interface IDeliveryManMovementModule : IInitializable<Vector3>
    {
        public void Update();
        public void FixedUpdate();
        public void SetDestination(Vector3 destination);
        public void ActivateNavMeshAgent(bool value);
    }
    
    public class DeliveryManMovementModule : MovementModuleWithNavMeshAgent, IDeliveryManMovementModule
    {
        private readonly IDeliveryManStatsModule _deliveryManStatModule;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _deliveryManTransform;

        private Vector3 _destination;
        private float _movementSpeed => _deliveryManStatModule.MovementSpeed;

        public DeliveryManMovementModule(DeliveryMan guest, IDeliveryManStatsModule deliveryManStatsModule)
        {
            _deliveryManStatModule = deliveryManStatsModule;
            _deliveryManTransform = guest.transform;

            _navMeshAgent = guest.GetComponent<NavMeshAgent>();

            // NavMeshAgent 설정 추가
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.autoTraverseOffMeshLink = false; // 자동 오프메시 링크 이동 비활성화
            _navMeshAgent.autoBraking = false; // 목적지 도착 시 자동 정지 비활성화
            _navMeshAgent.stoppingDistance = 0.1f; // 정지 거리를 작게 설정
            _navMeshAgent.acceleration = 120f;
        }

        public void Initialize(Vector3 startPosition)
        {
            _navMeshAgent.speed = _movementSpeed;
                
            if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _deliveryManTransform.position = hit.position;
            }
            
            ActivateNavMeshAgent(false);
        }

        public void SetDestination(Vector3 destination)
        {
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _destination = hit.position;
                
                if (_navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.ResetPath();
            
                    var path = new NavMeshPath();
            
                    if (NavMesh.CalculatePath(_deliveryManTransform.position, destination, NavMesh.AllAreas, path))
                    {
                        _navMeshAgent.SetPath(path);
                        ActivateNavMeshAgent(true);
                    }   
                }
            }
        }

        public void Update()
        {
            if (Vector3.Distance(_deliveryManTransform.position, _destination) > 0.5f)
            {
                SetDestination(_destination);
            }
        }

        public void FixedUpdate()
        {
            if (_navMeshAgent != null && _navMeshAgent.isStopped != true && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                // 현재 위치에서 목적지까지 경로 확인
                var path = new NavMeshPath();
                
                if (NavMesh.CalculatePath(_deliveryManTransform.position, _destination, NavMesh.AllAreas, path))
                {
                    _navMeshAgent.SetPath(path);
                }
            }
        }

        public void ActivateNavMeshAgent(bool value)
        {
            _navMeshAgent.isStopped = !value;
        }
    }
}