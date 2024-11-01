using System.Collections;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Managers;
using Units.Modules.FSMModules.Units;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Modules.MovementModules.Units
{
    public interface IPlayerMovementModule : IInitializable
    {
        void Update();
        void FixedUpdate();
        void HandleOnHit();
    }

    public class PlayerMovementModule : MovementModuleWithoutNavMeshAgent, IPlayerMovementModule
    {
        protected override CapsuleCollider2D capsuleCollider2D { get; }
        
        private readonly IMovementProperty _playerStatsModule;
        private readonly CreatureStateMachine _creatureStateMachine;
        private readonly Joystick _joystick;
        private readonly Transform _playerTransform;
        private readonly Transform _spriteTransform;
        
        private float _movementSpeed => _isSlowed ? _playerStatsModule.MovementSpeed * 0.2f : _playerStatsModule.MovementSpeed;
        private bool _isFacingRight = true;
        private bool _isMoving;
        private bool _isSlowed;
        private Coroutine _slowCoroutine;

        public PlayerMovementModule(
            Player player,
            IPlayerStatsModule playerStatsModule,
            CreatureStateMachine creatureStateMachine,
            Joystick joystick,
            Transform spriteTransform)
        {
            _playerStatsModule = playerStatsModule;
            _creatureStateMachine = creatureStateMachine;
            _joystick = joystick;
            _playerTransform = player.transform;
            _spriteTransform = spriteTransform;
            
            capsuleCollider2D = _playerTransform.GetComponent<CapsuleCollider2D>();
        }

        public void Initialize()
        {
            UpdateMovementFlag();
            UpdateStateMachine();
        }

        public void Update()
        {
            Vector2 direction = _joystick.direction;
            
            switch (direction.x)
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
            Vector3 moveDirection = _joystick.direction.normalized * (_movementSpeed * Time.deltaTime);
            MoveWithCollision(_playerTransform, moveDirection, ref moveDirection);
            HandleStateUpdate();
        }
        
        protected override bool HandleCollision(CapsuleCollider2D collider, Vector3 originalPosition, ref Vector3 move, ref Vector3 direction)
        {
            Vector3 colliderPosition = originalPosition + (Vector3)collider.offset;
            RaycastHit2D hit = Physics2D.CircleCast(colliderPosition, collider.size.y / 2, move.normalized, move.magnitude, collisionLayerMask);
            if (hit.collider != null)
            {
                direction = Vector3.Reflect(direction, hit.normal);
                return false;
            }
            return true;
        }

        private void FlipSprite(bool faceRight)
        {
            _isFacingRight = faceRight;
            Vector3 scale = _spriteTransform.localScale;
            scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _spriteTransform.localScale = scale;
        }

        private void HandleStateUpdate()
        {
            UpdateMovementFlag();
            UpdateStateMachine();
        }

        private void UpdateMovementFlag() => _isMoving = _joystick.direction != Vector2.zero;

        private void UpdateStateMachine()
        {
            _creatureStateMachine.ChangeState(_isMoving ? _creatureStateMachine.CreatureRunState : _creatureStateMachine.CreatureIdleState);
        }

        public void HandleOnHit()
        {
            if (_slowCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_slowCoroutine);
            _slowCoroutine = CoroutineManager.Instance.StartCoroutine(SlowDownTemporarily());
        }

        private IEnumerator SlowDownTemporarily()
        {
            _isSlowed = true;
            yield return new WaitForSeconds(0.5f);
            _isSlowed = false;
        }
    }
}