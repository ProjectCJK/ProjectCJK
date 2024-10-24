using System;
using Managers;
using Units.Modules.CollisionModules.Abstract;
using Units.Stages.Units.Buildings.Modules;
using UnityEngine;

namespace Units.Modules.CollisionModules.Units
{
    public interface IPlayerCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Action<bool> OnTriggerHuntingZone;
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
        public void Update();  // Player의 Update에서 호출할 메서드
    }

    public class PlayerCollisionModule : CollisionModule, IPlayerCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Action<bool> OnTriggerHuntingZone;

        private bool _isInTriggerZone;
        private Transform _targetTransform;

        private const float _waitingTime = 1.0f;
        private float _elapsedTime;  // 시간 측정 변수

        public void OnTriggerEnter2D(Collider2D other)
        {
            switch (CheckLayer(other.gameObject.layer))
            {
                case ECollisionType.None:
                    break;
                case ECollisionType.TradeZone:
                    Debug.Log("[OnTriggerEnter2D] Entered TradeZone");
                    _isInTriggerZone = true;
                    _targetTransform = other.transform;
                    _elapsedTime = 0f;  // 시간 초기화
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

        // Player의 Update에서 호출되는 메서드
        public void Update()
        {
            if (_isInTriggerZone && _targetTransform != null)
            {
                _elapsedTime += Time.deltaTime;
                if (_elapsedTime >= _waitingTime)
                {
                    OnWaitCompleted(true);
                    _elapsedTime = 0f;  // 시간 초기화
                }
            }
        }

        private void OnWaitCompleted(bool success)
        {
            Debug.Log($"[OnWaitCompleted] Completed with success: {success}");

            if (success && _isInTriggerZone)
            {
                var tradeZone = _targetTransform?.GetComponent<IInteractionTrade>();
                if (tradeZone != null)
                {
                    Debug.Log($"[OnWaitCompleted] Entering trade zone: {tradeZone}");
                    OnTriggerTradeZone?.Invoke(tradeZone, true);
                }
                else
                {
                    Debug.LogWarning("[OnWaitCompleted] InteractionTrade component is null or _targetTransform is null.");
                }
            }
            else
            {
                Debug.LogWarning("[OnWaitCompleted] Failed to enter trigger zone or _isInTriggerZone is false.");
            }
        }

        private void ResetTriggerState()
        {
            Debug.Log($"[ResetTriggerState] Exit trade zone: {_targetTransform?.GetComponent<IInteractionTrade>()}");

            if (_targetTransform != null) // null 체크 추가
            {
                Debug.Log($"[ResetTriggerState] Resetting _targetTransform and triggering OnTriggerTradeZone with false.");
                OnTriggerTradeZone?.Invoke(_targetTransform.GetComponent<IInteractionTrade>(), false);
            }

            _isInTriggerZone = false;
            _targetTransform = null;
            _elapsedTime = 0f;  // 시간 초기화

            // 현재 상태를 로그로 출력
            Debug.Log($"[ResetTriggerState] _isInTriggerZone: {_isInTriggerZone}, _targetTransform: {_targetTransform}");
        }
    }
}