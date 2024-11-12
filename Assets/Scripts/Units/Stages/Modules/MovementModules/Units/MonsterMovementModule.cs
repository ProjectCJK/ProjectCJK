using System.Collections;
using Interfaces;
using Managers;
using Units.Stages.Modules.FSMModules.Units.Creature;
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
        private const float SlowDuration = 0.5f;
        private const float DirectionChangeInterval = 3.0f;
        private const float DetectionRange = 2.0f;
        private const float IdleProbability = 0.3f;
        private static readonly int Encounter = Animator.StringToHash("Encounter");

        private readonly CreatureStateMachine _creatureStateMachine;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly int _monsterCollisionLayerMask = LayerMaskParserModule.MonsterCollisionLayerMask;
        private readonly IMonsterStatsModule _monsterStatModule;
        private readonly Transform _spriteTransform;

        private Coroutine _encounterCoroutine;
        private Coroutine _waitCoroutine;
        private bool _encounterTrigger;
        private bool _isFacingRight = true;
        private bool _isMoving;
        private bool _isPatrolling = true;
        private bool _isSlowed;
        private Vector2 _moveDirection;
        private float _nextDirectionChangeTime;
        private Transform _target;

        public bool hitTrigger { get; set; }

        public MonsterMovementModule(
            Monster monster,
            IMonsterStatsModule monsterStatModule,
            CreatureStateMachine creatureStateMachine,
            Transform spriteTransform)
        {
            _monsterStatModule = monsterStatModule;
            _creatureStateMachine = creatureStateMachine;
            _rigidbody2D = monster.GetComponent<Rigidbody2D>();
            _spriteTransform = spriteTransform;
            BoxCollider2D = _rigidbody2D.GetComponent<BoxCollider2D>();
            
            SetCollisionLayerMask(LayerMaskParserModule.CollisionLayerMask | LayerMaskParserModule.MonsterCollisionLayerMask);
        }

        protected override BoxCollider2D BoxCollider2D { get; }

        private float MovementSpeed => _isSlowed ? _monsterStatModule.MovementSpeed * 0.2f :
            _encounterTrigger ? _monsterStatModule.MovementSpeed * 2f : _monsterStatModule.MovementSpeed;

        public void Initialize()
        {
            ResetState();
            SetRandomDirection();
            UpdateStateMachine();
        }

        public void Update()
        {
            if (hitTrigger)
            {
                hitTrigger = false;
                StartSlowEffect();
                UpdateStateMachine();
            }
            else if (!_encounterTrigger && !hitTrigger)
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

            switch (_moveDirection.x)
            {
                case > 0 when !_isFacingRight:
                    FlipSprite(true);
                    break;
                case < 0 when _isFacingRight:
                    FlipSprite(false);
                    break;
            }
        }

        public void FixedUpdate()
        {
            if (_encounterTrigger || (_isPatrolling && !hitTrigger))
            {
                Vector2 moveDirection = _moveDirection * (MovementSpeed * Time.fixedDeltaTime);
                MoveWithCollision(_rigidbody2D, moveDirection, ref _moveDirection);
                UpdateStateMachine();
            }
        }

        private void ResetState()
        {
            hitTrigger = false;
            _target = null;
            SetEncounterTrigger(false);
            _isSlowed = false;
            _isMoving = false;
        }

        private void SetRandomDirection()
        {
            _moveDirection = Random.insideUnitCircle.normalized;
            _nextDirectionChangeTime = Time.time + DirectionChangeInterval;
            _isPatrolling = true;
            _isMoving = true;
            UpdateStateMachine();
        }

        private void StartWaiting()
        {
            _isPatrolling = false;
            _isMoving = false;
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
            Vector3 direction = (Vector3) _rigidbody2D.position + _rigidbody2D.transform.right * DetectionRange;
            RaycastHit2D hit = Physics2D.CircleCast(_rigidbody2D.position, BoxCollider2D.size.y * 4,
                direction.normalized, DetectionRange, LayerMaskParserModule.UnitLayerMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                _target = hit.collider.transform;
                return true;
            }

            return false;
        }

        private void StartEncounter()
        {
            SetEncounterTrigger(true);
            _moveDirection = (_rigidbody2D.position - (Vector2)_target.position).normalized;
            _isMoving = true;
            UpdateStateMachine();
            DrawDirectionRay();

            if (_encounterCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_encounterCoroutine);

            _encounterCoroutine = CoroutineManager.Instance.StartCoroutine(EncounterRoutine());
        }

        private IEnumerator EncounterRoutine()
        {
            yield return new WaitForSeconds(3f);
            SetEncounterTrigger(false);
            SetRandomDirection();
        }

        private void StartSlowEffect()
        {
            if (_isSlowed) return;

            _isSlowed = true;
            CoroutineManager.Instance.StartCoroutine(ResetSpeedAfterDelay());
        }

        private void FlipSprite(bool faceRight)
        {
            _isFacingRight = faceRight;
            Vector3 scale = _spriteTransform.localScale;
            scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _spriteTransform.localScale = scale;
        }

        private IEnumerator ResetSpeedAfterDelay()
        {
            yield return new WaitForSeconds(SlowDuration);
            _isSlowed = false;
        }

        private void UpdateStateMachine()
        {
            _creatureStateMachine.ChangeState(_isMoving
                ? _creatureStateMachine.CreatureRunState
                : _creatureStateMachine.CreatureIdleState);
        }

        private void DrawDirectionRay()
        {
            Color rayColor = _encounterTrigger ? Color.red : Color.green;
            Debug.DrawRay(_rigidbody2D.position, _moveDirection * 2f, rayColor, 0.1f);
        }

        private void SetEncounterTrigger(bool value)
        {
            _encounterTrigger = value;
            _creatureStateMachine.Creature.Animator.SetBool(Encounter, value);
        }
    }
}