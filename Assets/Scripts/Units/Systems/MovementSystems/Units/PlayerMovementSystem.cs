using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Modules;
using Units.Buildings.Interfaces;
using Units.Creatures.Interfaces;
using Units.Systems.MovementSystems.Abstract;
using Unity.VisualScripting;
using UnityEngine;
using IInitializable = Interfaces.IInitializable;

namespace Units.Systems.MovementSystems.Units
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovementSystem : BaseMovementSystem, IReferenceRegisterable<IMovementProperty>, IInitializable
    {
        public event Action<GameObject> OnTrade;
        
        [SerializeField] private Joystick _joystick;
        
        private IMovementProperty _movementProperty;
        private GameObject _currentInteractionTarget;
        private Coroutine _interactionCoroutine;
        
        private Rigidbody2D _rigidbody2D;
        
        
        private float _movementSpeed => _movementProperty.MovementSpeed;
        private float _waitingTime => _movementProperty.WaitingTime;
        private bool _isMoving;
        private bool _isInTriggerZone;

        #region 초기화
        
        public void RegisterReference(IMovementProperty movementProperty)
        {
            _movementProperty = movementProperty;
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        public void Initialize()
        {
            UpdateMovementFlag();
        }
        
        #endregion

        #region 이동 로직

        private void FixedUpdate()
        {
            Move();
            UpdateMovementFlag();
            CheckInteractionCondition();
        }

        private void Move()
        {
            Vector2 direction = _joystick.direction.normalized;
            Vector2 currentPosition = _rigidbody2D.position;
            Vector2 movement = direction * (_movementSpeed * Time.deltaTime);
            
            _rigidbody2D.MovePosition(currentPosition + movement); 
        }

        private void UpdateMovementFlag()
        {
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
                    case true when _interactionCoroutine != null:
                        StopCoroutine(_interactionCoroutine);
                        _interactionCoroutine = null;
                        break;
                    case false when _interactionCoroutine == null:
                        _interactionCoroutine = StartCoroutine(CoroutineManager.Timer(_waitingTime, OnWaitCompleted));
                        break;
                }
            }
        }

        #endregion

        #region 탐지 로직

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                _isInTriggerZone = true;
                _currentInteractionTarget = other.gameObject;
                StartInteractionCoroutine();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                ResetTriggerState();
            }
        }

        #endregion
        
        private void StartInteractionCoroutine()
        {
            _interactionCoroutine = StartCoroutine(CoroutineManager.Timer(_waitingTime, OnWaitCompleted));
        }
        
        private void OnWaitCompleted(bool success)
        {
            if (success && _isInTriggerZone && !_isMoving)
            {
                OnTrade?.Invoke(_currentInteractionTarget);
            }
        }

        private void ResetTriggerState()
        {
            _isInTriggerZone = false;
            _currentInteractionTarget = null;

            if (_interactionCoroutine != null)
            {
                StopCoroutine(_interactionCoroutine);
                _interactionCoroutine = null;
            }
        }
    }
}