using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Units.Modules.FSMModules.Units;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Stages.Creatures.Units;
using UnityEngine;

namespace Units.Modules.MovementModules.Units
{
    public interface IPlayerMovementModule : IInitializable
    {
        public void Move();
        public void FlipSprite();
    }
    
    public class PlayerMovementModule : MovementModule, IPlayerMovementModule
    {
        private readonly IMovementProperty _movementProperty;
        private readonly CreatureStateMachine _creatureStateMachine;
        private readonly Joystick _joystick;
        private readonly Rigidbody2D _playerRigidbody2D;
        private readonly Transform _spriteTransform;
        
        private float _movementSpeed => _movementProperty.MovementSpeed;
        private bool _isMoving;
        private bool _isFacingRight = true;

        public PlayerMovementModule(
            IPlayerStatsModule playerStatsModule,
            CreatureStateMachine creatureStateMachine,
            Joystick joystick,
            Rigidbody2D playerRigidbody2D,
            Transform spriteTransform
        )
        {
            _movementProperty = playerStatsModule;
            _creatureStateMachine = creatureStateMachine;
            _joystick = joystick;
            _playerRigidbody2D = playerRigidbody2D;
            _spriteTransform = spriteTransform;
        }

        public override void Initialize()
        {
            UpdateMovementFlag();
            UpdateStateMachine();
        }

        public void FlipSprite()
        {
            switch (_joystick.direction.x)
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
            scale.x = faceRight ? 1 : -1;
            _spriteTransform.localScale = scale;
        }

        public void Move()
        {
            HandleMovement();
            HandleStateUpdate();
        }

        private void HandleMovement()
        {
            Vector2 newDirection = CalculateDirection();
            MovePosition(newDirection);
        }

        public void MovePosition(Vector2 direction)
        {
            if (direction != _playerRigidbody2D.position)
            {
                _playerRigidbody2D.MovePosition(direction);
            }
        }

        private void HandleStateUpdate()
        {
            UpdateMovementFlag();
            UpdateStateMachine();
        }

        private Vector2 CalculateDirection()
        {
            Vector2 currentDirection = _joystick.direction.normalized;
            Vector2 currentPosition = _playerRigidbody2D.position;
            Vector2 movement = currentDirection * (_movementSpeed * Time.deltaTime);
            return currentPosition + movement;
        }

        private void UpdateMovementFlag()
        {
            _isMoving = _joystick.direction != Vector2.zero;
        }

        private void UpdateStateMachine()
        {
            if (_isMoving)
            {
                _creatureStateMachine.ChangeState(_creatureStateMachine.CreatureRunState);
            }
            else
            {
                _creatureStateMachine.ChangeState(_creatureStateMachine.CreatureIdleState);
            }
        }
    }
}