using System;
using System.Collections;
using Managers;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Modules.MovementModules.Units
{
    public interface IMonsterMovementModule : IInitializable
    {
        void HandleOnPlayerEncounter(bool value, Transform target);
        bool hit { get; set; }
        void Update();
        void FixedUpdate();
    }
    
    public class MonsterMovementModule : IMonsterMovementModule
    {
        public bool encountered;
        public bool hit { get; set; }

        private readonly IMonsterStatsModule _monsterStatModule;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Transform _guestTransform;

        private float _movementSpeed => _monsterStatModule.MovementSpeed;
        private Transform _target;
        private Vector3 _destination;

        private float _slowDuration = 3.0f; // 피격 상태 지속 시간 (조절 가능)
        private float _reducedSpeedFactor = 0.1f; // 피격 상태 이동 속도 감소 비율
        private bool _isSlowed; // 피격 상태 여부

        public MonsterMovementModule(Monster monster, IMonsterStatsModule monsterStatModule)
        {
            _monsterStatModule = monsterStatModule;
            _guestTransform = monster.transform;

            _navMeshAgent = monster.GetComponent<NavMeshAgent>();

            // NavMeshAgent 설정 추가
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.autoTraverseOffMeshLink = false;
            _navMeshAgent.autoBraking = false;
            _navMeshAgent.stoppingDistance = 0.1f;
            _navMeshAgent.acceleration = 120f;
        }

        public void Initialize()
        {
            _navMeshAgent.speed = _movementSpeed;
            hit = false;
            _target = null;
            encountered = false;
            _isSlowed = false;
        }

        public void Update()
        {
            if (hit)
            {
                hit = false;
                StartSlowEffect();
            }

            if (encountered && _target != null)
            {
                MoveAwayFromTarget();
            }
            else if (!encountered && !hit)
            {
                Patrol();
            }
        }

        public void FixedUpdate()
        {
            if (_navMeshAgent != null && !_navMeshAgent.isStopped && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(_guestTransform.position, _destination, NavMesh.AllAreas, path))
                {
                    _navMeshAgent.SetPath(path);
                }
            }
        }

        private void Patrol()
        {
            if (!_navMeshAgent.hasPath || _navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance)
            {
                SetRandomPatrolDestination();
            }
        }

        private void SetRandomPatrolDestination()
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 5f;
            randomDirection += _guestTransform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _destination = hit.position;
                _navMeshAgent.SetDestination(_destination);
            }
        }

        private void MoveAwayFromTarget()
        {
            Vector3 directionAway = (_guestTransform.position - _target.position).normalized;
            Vector3 targetPosition = _guestTransform.position + directionAway * 5f;

            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _destination = hit.position;
                _navMeshAgent.SetDestination(_destination);
            }
        }

        private void StartSlowEffect()
        {
            if (!_isSlowed)
            {
                _isSlowed = true;
                _navMeshAgent.speed = _movementSpeed * _reducedSpeedFactor;
                CoroutineManager.Instance.StartCoroutine(ResetSpeedAfterDelay());
            }
        }

        private IEnumerator ResetSpeedAfterDelay()
        {
            yield return new WaitForSeconds(_slowDuration);
            _navMeshAgent.speed = _movementSpeed;
            _isSlowed = false;
        }

        public void HandleOnPlayerEncounter(bool value, Transform target)
        {
            encountered = value;
            _target = target;
        }
    }
}