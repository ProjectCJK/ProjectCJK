using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Managers;
using Modules;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Units;
using UnityEngine;
using IInitializable = Interfaces.IInitializable;

namespace Units.Modules.MovementModules.Units
{
    public interface IPlayerMovementModule : IRegisterReference<IMovementProperty, Joystick>, IInitializable
    {
        public event Action<Transform, bool> OnTriggerInteractionZone;
    }
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovementModule : MovementModule, IPlayerMovementModule
    {
        public event Action<Transform, bool> OnTriggerInteractionZone;
        
        private IMovementProperty _movementProperty;
        
        private Joystick _joystick;
        private Coroutine _interactionCoroutine;
        private Rigidbody2D _rigidbody2D;
        private Transform _targetTransform;
        
        private float _movementSpeed => _movementProperty.MovementSpeed;
        private float _waitingTime => _movementProperty.WaitingTime;
        private bool _isMoving;
        private bool _isInTriggerZone;
        
        public void RegisterReference(IMovementProperty movementProperty, Joystick joystick)
        {
            _movementProperty = movementProperty;
            _joystick = joystick;
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            
            UpdateMovementFlag();
        }

        private void FixedUpdate()
        {
            MovePosition(CalculateDirection());
            UpdateMovementFlag();
            CheckInteractionCondition();
        }

        private Vector2 CalculateDirection()
        {
            Vector2 direction = _joystick.direction.normalized;
            Vector2 currentPosition = _rigidbody2D.position;
            Vector2 movement = direction * (_movementSpeed * Time.deltaTime);

            return currentPosition + movement;
        }

        private void MovePosition(Vector2 movement)
        {
            _rigidbody2D.MovePosition(movement); 
        }

        private void UpdateMovementFlag()
        {
            if (_joystick == null)
            {
                _isMoving = false;
            }
            
            _isMoving = _isMoving switch
            {
                true when _joystick.direction == Vector2.zero => false,
                false when _joystick.direction != Vector2.zero => true,
                _ => _isMoving
            };
        }
        
        private void CheckInteractionCondition()
        {
            if (_isInTriggerZone)
            {
                switch (_isMoving)
                {
                    case false when _interactionCoroutine == null:
                        _interactionCoroutine = StartCoroutine(CoroutineManager.Timer(_waitingTime, OnWaitCompleted));
                        break;
                    case true when _interactionCoroutine != null:
                        StopCoroutine(_interactionCoroutine);
                        _interactionCoroutine = null;
                        break;
                }
            }
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"(크리처) InteractionZone Trigger Enter 체크 : 'other.gameObject.layer == InteractionTradeLayerMask || other.gameObject.layer == InteractionUpgradeLayerMask' => {other.gameObject.layer == InteractionTradeLayerMask} || {other.gameObject.layer == InteractionUpgradeLayerMask}");
            
            if (other.gameObject.layer == InteractionTradeLayerMask || other.gameObject.layer == InteractionUpgradeLayerMask)
            {
                _isInTriggerZone = true;
                _targetTransform = other.transform;
                StartInteractionCoroutine();
            }
        }
        
        public void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log($"(크리처) InteractionZone Trigger Exit 체크 : 'other.gameObject.layer == InteractionTradeLayerMask || other.gameObject.layer == InteractionUpgradeLayerMask' => {other.gameObject.layer == InteractionTradeLayerMask} || {other.gameObject.layer == InteractionUpgradeLayerMask}");
            
            if (other.gameObject.layer == InteractionTradeLayerMask || other.gameObject.layer == InteractionUpgradeLayerMask)
            {
                ResetTriggerState();
            }
        }

        private void StartInteractionCoroutine()
        {
            _interactionCoroutine = StartCoroutine(CoroutineManager.Timer(_waitingTime, OnWaitCompleted));
        }
        
        private void OnWaitCompleted(bool success)
        {
            if (success && _isInTriggerZone && !_isMoving)
            {
                OnTriggerInteractionZone?.Invoke(_targetTransform, true);
            }
        }

        private void ResetTriggerState()
        {
            OnTriggerInteractionZone?.Invoke(_targetTransform, false);
            
            if (_interactionCoroutine != null)
            {
                StopCoroutine(_interactionCoroutine);
                _interactionCoroutine = null;
            }
            
            _isInTriggerZone = false;
            _targetTransform = null;
        }
    }
}