using System.Collections;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Managers;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Modules.MovementModules.Units
{
    public interface IPlayerMovementModule : IInitializable
    {
        void Update();
        void FixedUpdate();
        void HandleOnHit();
    }

    public class PlayerMovementModule : MovementModuleWithoutNavMeshAgent, IPlayerMovementModule
    {
        private readonly CreatureStateMachine _creatureStateMachine;
        private readonly Joystick _joystick;
        private readonly IMovementProperty _playerStatsModule;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly Transform _spriteTransform;
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
            _rigidbody2D = player.GetComponent<Rigidbody2D>();
            _spriteTransform = spriteTransform;
            BoxCollider2D = _rigidbody2D.GetComponent<BoxCollider2D>();
            
            SetCollisionLayerMask(LayerMaskParserModule.CollisionLayerMask);
        }

        protected override BoxCollider2D BoxCollider2D { get; }

        private float MovementSpeed => _isSlowed ? _playerStatsModule.MovementSpeed * 0.2f : _playerStatsModule.MovementSpeed;

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
            Vector2 moveDirection = _joystick.direction.normalized * (MovementSpeed * Time.fixedDeltaTime);
            MoveWithCollision(_rigidbody2D, moveDirection, ref moveDirection);
    
            UpdateMovementFlag();
            UpdateStateMachine();
        }

        public void HandleOnHit()
        {
            if (_slowCoroutine != null)
                CoroutineManager.Instance.StopCoroutine(_slowCoroutine);
            _slowCoroutine = CoroutineManager.Instance.StartCoroutine(SlowDownTemporarily());
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
            _isMoving = _joystick.direction != Vector2.zero;
        }

        private void UpdateStateMachine()
        {
            _creatureStateMachine.ChangeState(_isMoving
                ? _creatureStateMachine.CreatureRunState
                : _creatureStateMachine.CreatureIdleState);
        }

        private IEnumerator SlowDownTemporarily()
        {
            _isSlowed = true;
            yield return new WaitForSeconds(0.5f);
            _isSlowed = false;
        }
    }
}