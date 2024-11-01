using System;
using System.Collections;
using Interfaces;
using Managers;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Modules.MovementModules.Units
{
    public interface IMonsterMovementModule : IInitializable
    {
        bool hitTrigger { get; set; }
        void Update();
        void FixedUpdate();
    }

    public class MonsterMovementModule : MovementModule, IMonsterMovementModule
    {
        public bool encounterTrigger;
        public bool hitTrigger { get; set; }

        private readonly IMonsterStatsModule _monsterStatModule;
        private readonly Transform _monsterTransform;
        private readonly CapsuleCollider2D _monsterCollider;
        private readonly int _monsterCollisionLayerMask = LayerMaskParserModule.MonsterCollisionLayerMask;
        private readonly int _unitLayerMask = LayerMaskParserModule.UnitLayerMask;

        private readonly float _slowDuration = 0.5f;
        private readonly float _patrolChangeTime = 3.0f;
        private readonly float _detectionRange = 2.0f;

        private float _movementSpeed
        {
            get
            {
                if (_isSlowed)
                {
                    return _monsterStatModule.MovementSpeed * 0.2f;
                }

                if (encounterTrigger)
                {
                    return _monsterStatModule.MovementSpeed * 2f;
                }

                return _monsterStatModule.MovementSpeed;
            }
        }

        private Transform _target;
        private Vector3 _destination;
        private bool _isSlowed;
        private Vector3 _randomDirection;
        private float _nextPatrolChangeTime;

        public MonsterMovementModule(Monster monster, IMonsterStatsModule monsterStatModule)
        {
            _monsterStatModule = monsterStatModule;
            _monsterTransform = monster.transform;
            _monsterCollider = monster.GetComponent<CapsuleCollider2D>();
        }

        public void Initialize()
        {
            hitTrigger = false;
            _target = null;
            encounterTrigger = false;
            _isSlowed = false;

            SetRandomPatrolDestination();
            _nextPatrolChangeTime = Time.time + _patrolChangeTime;
        }

        public void Update()
        {
            if (!encounterTrigger && DetectPlayer())
            {
                encounterTrigger = true;
                MoveAwayFromTarget();
            }

            if (hitTrigger)
            {
                hitTrigger = false;
                StartSlowEffect();
            }

            if (!encounterTrigger && !hitTrigger)
            {
                Patrol();
            }
        }

        public void FixedUpdate()
        {
            MoveWithCollision((_destination - _monsterTransform.position).normalized * (_movementSpeed * Time.fixedDeltaTime));
        }

        private bool DetectPlayer()
        {
            Vector3 direction = _monsterTransform.position + _monsterTransform.right * _detectionRange;
            RaycastHit2D hit = Physics2D.CircleCast(_monsterTransform.position, _monsterCollider.size.y * 4, direction.normalized, _detectionRange, _unitLayerMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                _target = hit.collider.transform;
                return true;
            }

            return false;
        }

        private void Patrol()
        {
            if (Time.time >= _nextPatrolChangeTime || Vector3.Distance(_monsterTransform.position, _destination) < 0.1f)
            {
                SetRandomPatrolDestination();
                _nextPatrolChangeTime = Time.time + _patrolChangeTime;
            }
        }

        private void SetRandomPatrolDestination()
        {
            _randomDirection = UnityEngine.Random.insideUnitSphere * 5f;
            _randomDirection.z = 0;
            _destination = _monsterTransform.position + _randomDirection;
        }

        private void MoveAwayFromTarget()
        {
            Vector3 directionAway = (_monsterTransform.position - _target.position).normalized;
            _destination = _monsterTransform.position + directionAway * 5f;
        }

        private void StartSlowEffect()
        {
            if (!_isSlowed)
            {
                _isSlowed = true;
                CoroutineManager.Instance.StartCoroutine(ResetSpeedAfterDelay());
            }
        }

        private IEnumerator ResetSpeedAfterDelay()
        {
            yield return new WaitForSeconds(_slowDuration);
            _isSlowed = false;
        }

        private void MoveWithCollision(Vector3 move)
        {
            Vector3 originalPosition = _monsterTransform.position;

            var moveX = new Vector3(move.x, 0, 0);
            if (IsColliding(moveX, out RaycastHit2D hitX))
            {
                // X축 충돌이 발생하면 벽 바깥으로 밀어내기
                _monsterTransform.position = originalPosition + (Vector3)hitX.normal * 0.05f;
                moveX = Vector3.zero; // X축 이동 중단
            }

            _monsterTransform.position += moveX;

            var moveY = new Vector3(0, move.y, 0);
            if (IsColliding(moveY, out var hitY))
            {
                // Y축 충돌이 발생하면 벽 바깥으로 밀어내기
                _monsterTransform.position = originalPosition + (Vector3) hitY.normal * 0.05f;
                moveY = Vector3.zero; // Y축 이동 중단
            }

            _monsterTransform.position += moveY;
        }

        private bool IsColliding(Vector3 move, out RaycastHit2D hit)
        {
            Vector3 colliderPosition = _monsterTransform.position + (Vector3)_monsterCollider.offset;
            var combinedLayerMask = collisionLayerMask | _monsterCollisionLayerMask;

            hit = Physics2D.CircleCast(colliderPosition, _monsterCollider.size.y, move.normalized, move.magnitude, combinedLayerMask);

#if UNITY_EDITOR
            Color rayColor = hit.collider != null ? Color.red : Color.blue;
            Debug.DrawRay(colliderPosition, move.normalized * move.magnitude, rayColor);
            DebugDrawCircle(colliderPosition + move.normalized * move.magnitude, _monsterCollider.size.y, rayColor);
#endif

            return hit.collider != null;
        }

#if UNITY_EDITOR
        private static void DebugDrawCircle(Vector3 position, float radius, Color color)
        {
            const int segments = 20;
            const float increment = 360f / segments;
            var angle = 0f;

            Vector3 lastPoint = position +
                                new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) *
                                radius;
            angle += increment;

            for (var i = 0; i < segments; i++)
            {
                Vector3 nextPoint = position +
                                    new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) *
                                    radius;
                Debug.DrawLine(lastPoint, nextPoint, color);
                lastPoint = nextPoint;
                angle += increment;
            }
        }
#endif
    }
}
