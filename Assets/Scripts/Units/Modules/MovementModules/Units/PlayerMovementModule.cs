using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
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
        public void Update();
        public void FixedUpdate();
    }
    
    public class PlayerMovementModule : MovementModule, IPlayerMovementModule
    {
        private readonly IMovementProperty _playerStatsModule;
        private readonly CreatureStateMachine _creatureStateMachine;
        private readonly Joystick _joystick;
        private readonly CapsuleCollider2D _playerCollider;
        private readonly Transform _playerTransform;
        private readonly Transform _spriteTransform;
        private readonly ParticleSystem _walkParticles;
        private readonly Vector3 _faceScale;
        
        private float _movementSpeed => _playerStatsModule.MovementSpeed;
        private bool _isMoving;
        private bool _isFacingRight = true;

        private Vector2 _movementDirection; // 캐싱된 방향 정보

        public PlayerMovementModule(
            Player player,
            IPlayerStatsModule playerStatsModule,
            CreatureStateMachine creatureStateMachine,
            Joystick joystick,
            Transform spriteTransform)
        {
            _playerCollider = player.GetComponent<CapsuleCollider2D>();
            _playerTransform = player.transform;
            _playerStatsModule = playerStatsModule;
            _creatureStateMachine = creatureStateMachine;
            _joystick = joystick;
            _spriteTransform = spriteTransform;

            _faceScale = _spriteTransform.localScale;
        }

        public void Initialize()
        {
            UpdateMovementFlag();
            UpdateStateMachine();
        }

        public void Update()
        {
            switch (_movementDirection.x)
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
            HandleMovement();
            HandleStateUpdate();
        }

        private void FlipSprite(bool faceRight)
        {
            _isFacingRight = faceRight;

            Vector3 scale = _faceScale;
            scale.x = faceRight ? scale.x : scale.x * -1;
            _spriteTransform.localScale = scale;
        }

        private void HandleMovement()
        {
            _movementDirection = GetMovementDirection();
            MoveWithCollision(_movementDirection);
        }

        private Vector2 GetMovementDirection()
        {
            Vector2 currentDirection = _joystick.direction.normalized;
            return currentDirection * (_movementSpeed * Time.deltaTime);
        }

        private void MoveWithCollision(Vector3 move)
        {
            // X축 충돌 감지
            var moveX = new Vector3(move.x, 0, 0);
            if (!IsColliding(moveX))
            {
                // X축으로 충돌이 없을 때만 이동
                _playerTransform.position += moveX;
            }

            // Y축 충돌 감지
            var moveY = new Vector3(0, move.y, 0);
            if (!IsColliding(moveY))
            {
                // Y축으로 충돌이 없을 때만 이동
                _playerTransform.position += moveY;
            }
        }

        private bool IsColliding(Vector3 move)
        {
            // CapsuleCollider2D의 오프셋을 반영하여 충돌 검사 시작 위치 조정
            Vector3 colliderPosition = _playerTransform.position + (Vector3)_playerCollider.offset;

            // CircleCast 실행
            RaycastHit2D hit = Physics2D.CircleCast(colliderPosition, _playerCollider.size.y, move.normalized, move.magnitude, collisionLayerMask);

            // 레이 시각화
            Color rayColor = hit.collider != null ? Color.red : Color.blue; // 충돌 여부에 따라 색상 변경
            
#if UNITY_EDITOR
            Debug.DrawRay(colliderPosition, move.normalized * move.magnitude, rayColor);
            // Circle을 시각화하여 캐스트의 범위를 표시
            DebugDrawCircle(colliderPosition + move.normalized * move.magnitude, _playerCollider.size.y, rayColor);
#endif
            // 충돌 여부 반환
            return hit.collider != null;
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
        
        private void HandleStateUpdate()
        {
            UpdateMovementFlag();
            UpdateStateMachine();
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