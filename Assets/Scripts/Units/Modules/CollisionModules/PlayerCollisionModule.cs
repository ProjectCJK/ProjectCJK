using System;
using Managers;
using Units.Modules.CollisionModules.Abstract;
using UnityEngine;

namespace Units.Modules.CollisionModules
{
    public interface IPlayerCollisionModule
    {
        event Action<Transform, bool> OnTriggerInteractionZone;
        void OnTriggerEnter2D(Collider2D other);  // 충돌 정보를 받는 메서드
        void OnTriggerExit2D(Collider2D other);   // 충돌 정보를 받는 메서드
    }

    public class PlayerCollisionModule : CollisionModule, IPlayerCollisionModule
    {
        public event Action<Transform, bool> OnTriggerInteractionZone;

        private bool _isInTriggerZone;
        private Transform _targetTransform;
        private Coroutine _interactionCoroutine;

        private const float _waitingTime = 1.0f;

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (IsInteractionLayer(other.gameObject.layer))
            {
                _isInTriggerZone = true;
                _targetTransform = other.transform;
                StartInteractionCoroutine();
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (IsInteractionLayer(other.gameObject.layer))
            {
                ResetTriggerState();
            }
        }

        private bool IsInteractionLayer(int layer)
        {
            return layer == LayerMask.NameToLayer("InteractionTrade") || layer == LayerMask.NameToLayer("InteractionUpgrade");
        }

        private void StartInteractionCoroutine()
        {
            _interactionCoroutine = CoroutineManager.Instance.StartCoroutine(CoroutineManager.Timer(_waitingTime, OnWaitCompleted));
        }

        private void OnWaitCompleted(bool success)
        {
            if (success && _isInTriggerZone)
            {
                OnTriggerInteractionZone?.Invoke(_targetTransform, true);
            }
        }

        private void ResetTriggerState()
        {
            OnTriggerInteractionZone?.Invoke(_targetTransform, false);

            if (_interactionCoroutine != null)
            {
                CoroutineManager.Instance.StopCoroutine(_interactionCoroutine);
                _interactionCoroutine = null;
            }

            _isInTriggerZone = false;
            _targetTransform = null;
        }
    }
}