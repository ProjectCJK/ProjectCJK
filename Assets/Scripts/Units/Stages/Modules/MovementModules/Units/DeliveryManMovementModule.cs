using Interfaces;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Modules.MovementModules.Units
{
    public interface IDeliveryManMovementModule : IInitializable<Vector3>
    {
        void Update();
        void FixedUpdate();
        void SetDestination(Vector3 destination);
        void ActivateNavMeshAgent(bool value);
    }

    public class DeliveryManMovementModule : MovementModuleWithNavMeshAgent, IDeliveryManMovementModule
    {
        private readonly CreatureStateMachine _creatureStateMachine;
        private readonly IDeliveryManStatsModule _deliveryManStatModule;
        private readonly Transform _deliveryManTransform;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _spriteTransform;

        private Vector3 _destination;
        private bool _isFacingRight;
        private bool _isMoving;

        public DeliveryManMovementModule(DeliveryMan guest, IDeliveryManStatsModule deliveryManStatsModule,
            CreatureStateMachine creatureStateMachine, Transform spriteTransform)
        {
            _deliveryManStatModule = deliveryManStatsModule;
            _deliveryManTransform = guest.transform;
            _spriteTransform = spriteTransform;
            _creatureStateMachine = creatureStateMachine;
            _navMeshAgent = guest.GetComponent<NavMeshAgent>();

            // NavMeshAgent 설정 추가
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.autoTraverseOffMeshLink = false; // 자동 오프메시 링크 이동 비활성화
            _navMeshAgent.autoBraking = false; // 목적지 도착 시 자동 정지 비활성화
            _navMeshAgent.stoppingDistance = 0.5f; // 정지 거리를 작게 설정
            _navMeshAgent.acceleration = 120f;
        }

        private float _movementSpeed => _deliveryManStatModule.MovementSpeed;

        public void Initialize(Vector3 startPosition)
        {
            _navMeshAgent.speed = _movementSpeed;

            if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                _deliveryManTransform.position = hit.position;

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
            if (Vector3.Distance(_deliveryManTransform.position, _destination) > 0.5f) SetDestination(_destination);

            UpdateMovementFlag();
            UpdateStateMachine();
            HandleSpriteFlip();
        }

        public void FixedUpdate()
        {
            if (_navMeshAgent != null && !_navMeshAgent.isStopped && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                var path = new NavMeshPath();

                if (NavMesh.CalculatePath(_deliveryManTransform.position, _destination, NavMesh.AllAreas, path))
                    _navMeshAgent.SetPath(path);
            }
        }

        public void ActivateNavMeshAgent(bool value)
        {
            _navMeshAgent.isStopped = !value;
        }

        private void HandleSpriteFlip()
        {
            switch (_navMeshAgent.velocity.x)
            {
                case > 0 when !_isFacingRight:
                    FlipSprite(true);
                    break;
                case < 0 when _isFacingRight:
                    FlipSprite(false);
                    break;
            }
        }

        private void FlipSprite(bool faceRight)
        {
            _isFacingRight = faceRight;
            Vector3 scale = _spriteTransform.localScale;
            scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _spriteTransform.localScale = scale;
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
    }
}