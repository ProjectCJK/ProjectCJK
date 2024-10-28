using System;
using System.Collections.Generic;
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
        private readonly Stack<Transform> _tradeZones = new();
        
        private const float _waitingTime = 1.0f;
        private float _elapsedTime;  // 시간 측정 변수
        private Transform _previousTargetTransform; // 이전 트리거 존을 저장
        private bool _isWaitingForTrigger; // 대기 상태 여부

        public void OnTriggerEnter2D(Collider2D other)
        {
            switch (CheckLayer(other.gameObject.layer))
            {
                case ECollisionType.None:
                    break;
                case ECollisionType.TradeZone:
                    EnterTradeZone(other.transform);
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
                    ExitTradeZone(other.transform);
                    break;

                case ECollisionType.UpgradeZone:
                    break;

                case ECollisionType.HuntingZone:
                    OnTriggerHuntingZone?.Invoke(false);
                    break;
            }
        }

        // Player의 Update에서 호출되는 메서드
        public void Update()
        {
            if (_isWaitingForTrigger && _isInTriggerZone && _targetTransform != null)
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime >= _waitingTime)
                {
                    CompleteWaitForTrigger();
                }
            }
        }
        
        private void EnterTradeZone(Transform tradeZoneTransform)
        {
            if (_tradeZones.Count == 0 || _tradeZones.Peek() != tradeZoneTransform)
            {
                _tradeZones.Push(tradeZoneTransform);
                StartWaitingForTrigger(tradeZoneTransform);
            }
        }

        private void ExitTradeZone(Transform tradeZoneTransform)
        {
            if (_tradeZones.Count > 0 && _tradeZones.Peek() == tradeZoneTransform)
            {
                NotifyTradeZoneExit();
                _tradeZones.Pop();
                UpdateTargetTransform();
            }
        }

        private void StartWaitingForTrigger(Transform tradeZoneTransform)
        {
            _targetTransform = tradeZoneTransform;
            _isWaitingForTrigger = true;
            _elapsedTime = 0f;
        }

        private void CompleteWaitForTrigger()
        {
            _isWaitingForTrigger = false;
            _elapsedTime = 0f;

            var tradeZone = _targetTransform?.GetComponent<IInteractionTrade>();
            if (tradeZone != null)
            {
                Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {tradeZone}");
                OnTriggerTradeZone?.Invoke(tradeZone, true);
            }
            else
            {
                Debug.LogWarning("[CompleteWaitForTrigger] InteractionTrade component is null or _targetTransform is null.");
            }
        }

        private void UpdateTargetTransform()
        {
            if (_tradeZones.Count > 0)
            {
                StartWaitingForTrigger(_tradeZones.Peek());
            }
            else
            {
                ResetTriggerState();
            }
        }

        private void ResetTriggerState()
        {
            _targetTransform = null;
            _isWaitingForTrigger = false;
            _elapsedTime = 0f;
        }

        private void NotifyTradeZoneExit()
        {
            var tradeZone = _tradeZones.Peek()?.GetComponent<IInteractionTrade>();
            if (tradeZone != null)
            {
                Debug.Log($"[NotifyTradeZoneExit] Exiting trade zone: {tradeZone}");
                OnTriggerTradeZone?.Invoke(tradeZone, false);
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
    }
}