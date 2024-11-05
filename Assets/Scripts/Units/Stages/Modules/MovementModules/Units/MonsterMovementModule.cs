using System.Collections;
using Interfaces;
using Managers;
using Units.Stages.Modules.FSMModules.Units.Monsters;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Modules.MovementModules.Units
{
    public interface IMonsterMovementModule : IInitializable
    {
        bool hitTrigger { get; set; }
        void Update();
        void FixedUpdate();
    }

    public class MonsterMovementModule : MovementModuleWithoutNavMeshAgent, IMonsterMovementModule
    {
        public bool hitTrigger { get; set; }
        
        protected override CapsuleCollider2D capsuleCollider2D { get; }
        
        private readonly IMonsterStatsModule _monsterStatModule;
        private readonly MonsterStateMachine _monsterStateMachine;
        private readonly Transform _monsterTransform;

        private const float SlowDuration = 0.5f;
        private const float DirectionChangeInterval = 3.0f;
        private const float DetectionRange = 2.0f;
        private const float IdleProbability = 0.3f;

        private readonly int _monsterCollisionLayerMask = LayerMaskParserModule.MonsterCollisionLayerMask;
        
        private Transform _target;
        private Vector3 _moveDirection;
        private bool _isSlowed;
        private bool encounterTrigger;
        private bool _isPatrolling = true;
        private bool _isMoving; // 추가된 변수
        private float _nextDirectionChangeTime;
        private Coroutine _encounterCoroutine;
        private Coroutine _waitCoroutine;
        private float MovementSpeed => _isSlowed ? _monsterStatModule.MovementSpeed * 0.2f : encounterTrigger ? _monsterStatModule.MovementSpeed * 2f : _monsterStatModule.MovementSpeed;

        public MonsterMovementModule(
            Monster monster,
            IMonsterStatsModule monsterStatModule,
            MonsterStateMachine monsterStateMachine)
        {
            _monsterStatModule = monsterStatModule;
            _monsterStateMachine = monsterStateMachine;
            _monsterTransform = monster.transform;
            capsuleCollider2D = monster.GetComponent<CapsuleCollider2D>();
        }

        public void Initialize()
        {
            ResetState();
            SetRandomDirection();
            UpdateStateMachine();
        }

        private void ResetState()
        {
            hitTrigger = false;
            _target = null;
            encounterTrigger = false;
            _isSlowed = false;
            _isMoving = false; // 초기 상태 설정
        }

        public void Update()
        {
            if (hitTrigger)
            {
                hitTrigger = false;
                StartSlowEffect();
                UpdateStateMachine();
            }
            else if (!encounterTrigger && !hitTrigger)
            {
                if (DetectPlayer())
                {
                    StartEncounter();
                }
                else if (_isPatrolling && Time.time >= _nextDirectionChangeTime)
                {
                    if (Random.value < IdleProbability)
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
                Vector3 previousPosition = _monsterTransform.position;
                MoveWithCollision(_monsterTransform, _moveDirection * (MovementSpeed * Time.fixedDeltaTime), ref _moveDirection);
                _isMoving = _monsterTransform.position != previousPosition;
                UpdateStateMachine(); // 상태 업데이트
            }
            
            DrawDirectionRay();
        }

        protected override bool HandleCollision(CapsuleCollider2D collider, Vector3 originalPosition, ref Vector3 move, ref Vector3 direction)
        {
            Vector3 colliderPosition = originalPosition + (Vector3)collider.offset;
            RaycastHit2D hit = Physics2D.CircleCast(colliderPosition, collider.size.y / 2, move.normalized, move.magnitude, _monsterCollisionLayerMask | collisionLayerMask);
            if (hit.collider != null)
            {
                direction = Vector3.Reflect(direction, hit.normal);
                return false;
            }
            return true;
        }

        private void SetRandomDirection()
        {
            _moveDirection = Random.insideUnitCircle.normalized;
            _nextDirectionChangeTime = Time.time + DirectionChangeInterval;
            _isPatrolling = true;
            _isMoving = true; // 방향 설정 시 움직이는 상태로 변경
            UpdateStateMachine();
        }

        private void StartWaiting()
        {
            _isPatrolling = false;
            _isMoving = false; // 대기 상태에서는 멈춘 상태로 변경
            UpdateStateMachine();

            if (_waitCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_waitCoroutine);

            _waitCoroutine = CoroutineManager.Instance.StartCoroutine(WaitInPlace());
        }

        private IEnumerator WaitInPlace()
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            SetRandomDirection();
        }

        private bool DetectPlayer()
        {
            Vector3 direction = _monsterTransform.position + _monsterTransform.right * DetectionRange;
            RaycastHit2D hit = Physics2D.CircleCast(_monsterTransform.position, capsuleCollider2D.size.y * 4, direction.normalized, DetectionRange, LayerMaskParserModule.UnitLayerMask);

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
            _isMoving = true; // 도망 상태에서 움직임 활성화
            UpdateStateMachine();

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

        private void UpdateStateMachine()
        {
            _monsterStateMachine.ChangeState(_isMoving ? _monsterStateMachine.MonsterRunState : _monsterStateMachine.MonsterIdleState);
        }

        private void DrawDirectionRay()
        {
            Color rayColor = encounterTrigger ? Color.red : Color.green;
            Debug.DrawRay(_monsterTransform.position, _moveDirection * 2f, rayColor, 0.1f);
        }
    }
}