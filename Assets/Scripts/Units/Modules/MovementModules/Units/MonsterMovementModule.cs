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

        private Transform _target;
        private Vector3 _destination;
        private bool _isSlowed;
        private bool _isPatrolling;
        private float _nextPatrolChangeTime;
        private Coroutine _encounterCoroutine;

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

            StartPatrolling();
        }

        public void Update()
        {
            if (!encounterTrigger && !hitTrigger)
            {
                if (DetectPlayer())
                {
                    StartEncounter();
                }
                else
                {
                    UpdatePatrol();
                }
            }

            if (hitTrigger)
            {
                hitTrigger = false;
                StartSlowEffect();
            }
        }

        public void FixedUpdate()
        {
            if (_isPatrolling || encounterTrigger)
            {
                MoveWithCollision((_destination - _monsterTransform.position).normalized * (_movementSpeed * Time.fixedDeltaTime));
            }
        }

        private void StartPatrolling()
        {
            _isPatrolling = true;
            _destination = GetRandomPatrolPoint();
            _nextPatrolChangeTime = Time.time + UnityEngine.Random.Range(1f, _patrolChangeTime);
        }

        private void UpdatePatrol()
        {
            if (Time.time >= _nextPatrolChangeTime || Vector3.Distance(_monsterTransform.position, _destination) < 0.1f)
            {
                _destination = GetRandomPatrolPoint();
                _nextPatrolChangeTime = Time.time + UnityEngine.Random.Range(1f, _patrolChangeTime);
            }
        }

        private Vector3 GetRandomPatrolPoint()
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitCircle * 5f;
            randomDirection.z = 0;
            return _monsterTransform.position + randomDirection;
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

        private void StartEncounter()
        {
            encounterTrigger = true;
    
            // 타겟 반대 방향으로 3m 떨어진 위치를 도망 목적지로 설정
            Vector3 fleeDirection = (_monsterTransform.position - _target.position).normalized;
            _destination = _monsterTransform.position + fleeDirection * 3f;

            _monsterCollider.enabled = false;

            // 타겟 반대 방향으로의 도망 경로를 디버그 레이로 표시
            Debug.DrawRay(_monsterTransform.position, fleeDirection * 3f, Color.yellow, 3f);

            if (_encounterCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_encounterCoroutine);
    
            _encounterCoroutine = CoroutineManager.Instance.StartCoroutine(EncounterRoutine());
        }

        private IEnumerator EncounterRoutine()
        {
            yield return new WaitForSeconds(3f);
            encounterTrigger = false;
            _monsterCollider.enabled = true;
            StartPatrolling();
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

            // X축 이동
            var moveX = new Vector3(move.x, 0, 0);
            if (IsColliding(moveX, out RaycastHit2D hitX))
            {
                // 충돌 발생 시 충돌 방향 반사 및 위치 보정
                _monsterTransform.position = originalPosition + (Vector3)hitX.normal * 0.1f;
                moveX = Vector3.Reflect(moveX, hitX.normal); // 반사 방향으로 이동
            }
            _monsterTransform.position += moveX;

            // 이동 방향을 레이로 표시
            Debug.DrawRay(_monsterTransform.position, moveX, Color.green);

            // Y축 이동
            var moveY = new Vector3(0, move.y, 0);
            if (IsColliding(moveY, out RaycastHit2D hitY))
            {
                _monsterTransform.position = originalPosition + (Vector3)hitY.normal * 0.1f;
                moveY = Vector3.Reflect(moveY, hitY.normal);
            }
            _monsterTransform.position += moveY;

            // 이동 방향을 레이로 표시
            Debug.DrawRay(_monsterTransform.position, moveY, Color.green);
        }

        private bool IsColliding(Vector3 move, out RaycastHit2D hit)
        {
            Vector3 colliderPosition = _monsterTransform.position + (Vector3)_monsterCollider.offset;
            var combinedLayerMask = collisionLayerMask | _monsterCollisionLayerMask;

            hit = Physics2D.CircleCast(colliderPosition, _monsterCollider.size.y / 2, move.normalized, move.magnitude, combinedLayerMask);

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