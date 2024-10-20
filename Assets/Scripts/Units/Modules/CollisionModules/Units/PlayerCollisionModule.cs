using System;
using Managers;
using Units.Modules.CollisionModules.Abstract;
using UnityEngine;

namespace Units.Modules.CollisionModules
{
    public interface IPlayerCollisionModule
    {
        public event Action<Transform, bool> OnTriggerTradeZone;
        public event Action<bool> OnTriggerHuntingZone;
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
    }

    public class PlayerCollisionModule : CollisionModule, IPlayerCollisionModule
    {
        public event Action<Transform, bool> OnTriggerTradeZone;
        public event Action<bool> OnTriggerHuntingZone;

        private bool _isInTriggerZone;
        private Transform _targetTransform;
        private Coroutine _interactionCoroutine;

        private const float _waitingTime = 1.0f;

        public void OnTriggerEnter2D(Collider2D other)
        {
            switch (CheckLayer(other.gameObject.layer))
            {
                case ECollisionType.None:
                    break;
                case ECollisionType.TradeZone:
                    _isInTriggerZone = true;
                    _targetTransform = other.transform;
                    StartInteractionCoroutine();
                    break;
                case ECollisionType.UpgradeZone:
                    break;
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            switch (CheckLayer(other.gameObject.layer))
            {
                case ECollisionType.None:
                    break;
                case ECollisionType.HuntingZone:
                    OnTriggerHuntingZone?.Invoke(true);
                    break;
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            switch (CheckLayer(other.gameObject.layer))
            {
                case ECollisionType.None:
                    break;
                case ECollisionType.TradeZone:
                    ResetTriggerState();
                    break;
                case ECollisionType.UpgradeZone:
                    break;
                case ECollisionType.HuntingZone:
                    OnTriggerHuntingZone?.Invoke(false);
                    break;
            }
        }

        private ECollisionType CheckLayer(int layer)
        {
            return layer switch
            {
                _ when layer == tradeZoneLayerMask => ECollisionType.TradeZone,
                _ when layer == upgradeZoneLayerMask => ECollisionType.UpgradeZone,
                _ when layer == huntingZoneLayerMask => ECollisionType.HuntingZone,
                _ => ECollisionType.None
            };
        }

        private void StartInteractionCoroutine()
        {
            _interactionCoroutine = CoroutineManager.Instance.StartCoroutine(CoroutineManager.Timer(_waitingTime, OnWaitCompleted));
        }

        private void OnWaitCompleted(bool success)
        {
            if (success && _isInTriggerZone)
            {
                OnTriggerTradeZone?.Invoke(_targetTransform, true);
            }
        }

        private void ResetTriggerState()
        {
            OnTriggerTradeZone?.Invoke(_targetTransform, false);

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