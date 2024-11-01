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

        private const float SlowDuration = 0.5f;
        private const float DirectionChangeInterval = 3.0f;
        private const float DetectionRange = 2.0f;
        private const float IdleProbability = 0.3f;

        private Transform _target;
        private Vector3 _moveDirection;
        private bool _isSlowed;
        private bool _isPatrolling = true;
        private float _nextDirectionChangeTime;
        private Coroutine _encounterCoroutine;
        private Coroutine _waitCoroutine;

        private float MovementSpeed => _isSlowed ? _monsterStatModule.MovementSpeed * 0.2f :
                                  encounterTrigger ? _monsterStatModule.MovementSpeed * 2f :
                                                     _monsterStatModule.MovementSpeed;

        public MonsterMovementModule(Monster monster, IMonsterStatsModule monsterStatModule)
        {
            _monsterStatModule = monsterStatModule;
            _monsterTransform = monster.transform;
            _monsterCollider = monster.GetComponent<CapsuleCollider2D>();
        }

        public void Initialize()
        {
            ResetState();
            SetRandomDirection();
        }

        private void ResetState()
        {
            hitTrigger = false;
            _target = null;
            encounterTrigger = false;
            _isSlowed = false;
        }

        public void Update()
        {
            if (hitTrigger)
            {
                hitTrigger = false;
                StartSlowEffect();
            }
            else if (!encounterTrigger && !hitTrigger)
            {
                if (DetectPlayer())
                {
                    StartEncounter();
                }
                else if (_isPatrolling && Time.time >= _nextDirectionChangeTime)
                {
                    if (UnityEngine.Random.value < IdleProbability)
                        StartWaiting();
                    else
                        SetRandomDirection();
                }
            }
        }

        public void FixedUpdate()
        {
            if (encounterTrigger || (_isPatrolling && !hitTrigger))
            {
                MoveWithCollision(_moveDirection * (MovementSpeed * Time.fixedDeltaTime));
            }

            DrawDirectionRay();
        }

        private void SetRandomDirection()
        {
            _moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
            _nextDirectionChangeTime = Time.time + DirectionChangeInterval;
            _isPatrolling = true;
        }

        private void StartWaiting()
        {
            _isPatrolling = false;
            if (_waitCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_waitCoroutine);

            _waitCoroutine = CoroutineManager.Instance.StartCoroutine(WaitInPlace());
        }

        private IEnumerator WaitInPlace()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
            SetRandomDirection();
        }

        private bool DetectPlayer()
        {
            Vector3 direction = _monsterTransform.position + _monsterTransform.right * DetectionRange;
            RaycastHit2D hit = Physics2D.CircleCast(_monsterTransform.position, _monsterCollider.size.y * 4, direction.normalized, DetectionRange, LayerMaskParserModule.UnitLayerMask);

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
            _moveDirection = (_monsterTransform.position - _target.position).normalized;

            DrawDirectionRay();

            if (_encounterCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_encounterCoroutine);

            _encounterCoroutine = CoroutineManager.Instance.StartCoroutine(EncounterRoutine());
        }

        private IEnumerator EncounterRoutine()
        {
            yield return new WaitForSeconds(3f);
            encounterTrigger = false;
            SetRandomDirection();
        }

        private void StartSlowEffect()
        {
            if (_isSlowed) return;

            _isSlowed = true;
            CoroutineManager.Instance.StartCoroutine(ResetSpeedAfterDelay());
        }

        private IEnumerator ResetSpeedAfterDelay()
        {
            yield return new WaitForSeconds(SlowDuration);
            _isSlowed = false;
        }

        private void MoveWithCollision(Vector3 move)
        {
            Vector3 originalPosition = _monsterTransform.position;

            // X축 이동
            var moveX = new Vector3(move.x, 0, 0);
            if (HandleCollision(ref moveX, out var hitX))
            {
                _monsterTransform.position = originalPosition + (Vector3) hitX.normal * 0.1f;
                _moveDirection = Vector3.Reflect(_moveDirection, hitX.normal);
            }
            _monsterTransform.position += moveX;

            // Y축 이동
            var moveY = new Vector3(0, move.y, 0);
            if (HandleCollision(ref moveY, out var hitY))
            {
                _monsterTransform.position = originalPosition + (Vector3) hitY.normal * 0.1f;
                _moveDirection = Vector3.Reflect(_moveDirection, hitY.normal);
            }
            _monsterTransform.position += moveY;
        }

        private bool HandleCollision(ref Vector3 move, out RaycastHit2D hit)
        {
            Vector3 colliderPosition = _monsterTransform.position + (Vector3)_monsterCollider.offset;
            hit = Physics2D.CircleCast(colliderPosition, _monsterCollider.size.y / 2, move.normalized, move.magnitude, LayerMaskParserModule.MonsterCollisionLayerMask);

#if UNITY_EDITOR
            if (hit.collider != null)
            {
                Debug.DrawRay(colliderPosition, move.normalized * move.magnitude, Color.red);
                DebugDrawCircle(colliderPosition + move.normalized * move.magnitude, _monsterCollider.size.y, Color.red);
            }
#endif
            return hit.collider != null;
        }

        private void DrawDirectionRay()
        {
            Color rayColor = encounterTrigger ? Color.red : Color.green;
            Debug.DrawRay(_monsterTransform.position, _moveDirection * 2f, rayColor, 0.1f);
        }

#if UNITY_EDITOR
        private static void DebugDrawCircle(Vector3 position, float radius, Color color)
        {
            const int segments = 20;
            const float increment = 360f / segments;
            var angle = 0f;

            Vector3 lastPoint = position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
            angle += increment;

            for (var i = 0; i < segments; i++)
            {
                Vector3 nextPoint = position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
                Debug.DrawLine(lastPoint, nextPoint, color);
                lastPoint = nextPoint;
                angle += increment;
            }
        }
#endif
    }
}