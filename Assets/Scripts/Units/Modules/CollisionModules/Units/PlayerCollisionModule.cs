using System;
using System.Collections.Generic;
using Managers;
using Units.Modules.CollisionModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Stages.Units.Buildings.Modules;
using UnityEngine;

namespace Units.Modules.CollisionModules.Units
{
    public interface IPlayerCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Action<bool> OnTriggerHuntingZone;
        public void Update();
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
    }

    public class PlayerCollisionModule : CollisionModule, IPlayerCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Action<bool> OnTriggerHuntingZone;
        
        private readonly Queue<Transform> _tradeZones = new();
        private readonly float _waitingTime;
        
        private Transform _targetTransform;
        private Transform _previousTargetTransform; // 이전 트리거 존을 저장
        private float _elapsedTime;  // 시간 측정 변수
        private bool _isWaitingForTrigger; // 대기 상태 여부

        public PlayerCollisionModule(IInteractionProperty playerStatsModule)
        {
            _waitingTime = playerStatsModule.WaitingTime;
        }
        
        public void Update()
        {
            if (_isWaitingForTrigger && _targetTransform != null)
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime >= _waitingTime)
                {
                    CompleteWaitForTrigger();
                }
            }
        }

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
        
        private void EnterTradeZone(Transform tradeZoneTransform)
        {
            if (_tradeZones.Count == 0 || !ReferenceEquals(_tradeZones.Peek(), tradeZoneTransform))
            {
                _tradeZones.Enqueue(tradeZoneTransform);
                StartWaitingForTrigger(tradeZoneTransform);
            }
        }

        private void ExitTradeZone(Transform tradeZoneTransform)
        {
            if (_tradeZones.Count > 0 && ReferenceEquals(_tradeZones.Peek(), tradeZoneTransform))
            {
                NotifyTradeZoneExit();
                _tradeZones.Dequeue();
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
    }
}