using System;
using System.Collections.Generic;
using Units.Stages.Modules.CollisionModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.PaymentZones.Units;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Units;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.UnlockZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.UnlockZones.Units;
using Units.Stages.Units.Zones.Units.HuntingZones;
using UnityEngine;

namespace Units.Stages.Modules.CollisionModules.Units
{
    public interface IPlayerCollisionModule
    {
        public event Action<ITradeZone, bool> OnTriggerTradeZone;
        public event Action<IPaymentZone, bool> OnTriggerPaymentZone;
        public event Action<IUnlockZone, bool> OnTriggerUnlockZone;
        public event Action<bool> OnTriggerHuntingZone;
        public void Update();
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
    }

    public class PlayerCollisionModule : CollisionModule, IPlayerCollisionModule
    {
        public event Action<ITradeZone, bool> OnTriggerTradeZone;
        public event Action<IPaymentZone, bool> OnTriggerPaymentZone;
        public event Action<IUnlockZone, bool> OnTriggerUnlockZone;
        public event Action<bool> OnTriggerHuntingZone;
        
        private readonly ECreatureType _creatureType;
        private readonly Queue<ITradeZone> _tradeZones = new();
        private readonly float _waitingTime;
        
        private ITradeZone _tradeZone;
        private ITradeZone _previousTradeZone; // 이전 트리거 존을 저장
        
        private float _elapsedTime;  // 시간 측정 변수
        private bool _isWaitingForTrigger; // 대기 상태 여부

        public PlayerCollisionModule(IPlayerStatsModule playerStatsModule)
        {
            _creatureType = playerStatsModule.CreatureType;
            _waitingTime = playerStatsModule.WaitingTime;
        }
        
        public void Update()
        {
            if (_isWaitingForTrigger && _tradeZone != null)
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
                    if (other.transform.TryGetComponent(out IPlayerTradeZone currentTradeZone))
                    {
                        if (currentTradeZone.CheckAccessorPlayer()) EnterTradeZone(currentTradeZone);                        
                    }
                    break;
                
                case ECollisionType.UpgradeZone:
                    break;
                
                case ECollisionType.PaymentZone:
                    if (other.transform.TryGetComponent(out IPlayerPaymentZone currentPaymentZone))
                    {
                        OnTriggerPaymentZone?.Invoke(currentPaymentZone, true);                        
                    }
                    break;
                case ECollisionType.UnlockZone:
                    if (other.transform.TryGetComponent(out IPlayerUnlockZone currentUnlockZone))
                    {
                        OnTriggerUnlockZone?.Invoke(currentUnlockZone, true);                        
                    }
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
                    var currentHuntingZone = other.transform.GetComponentInParent<IHuntingZoneProperty>();
                    if (currentHuntingZone != null) OnTriggerHuntingZone?.Invoke(true);
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
                    if (other.transform.TryGetComponent(out IPlayerTradeZone currentTradeZone))
                    {
                        if (currentTradeZone.CheckAccessorPlayer()) ExitTradeZone(currentTradeZone);    
                    }
                    break;

                case ECollisionType.UpgradeZone:
                    break;

                case ECollisionType.HuntingZone:
                    var currentHuntingZone = other.transform.GetComponentInParent<IHuntingZoneProperty>();
                    if (currentHuntingZone != null) OnTriggerHuntingZone?.Invoke(false);
                    break;
                
                case ECollisionType.PaymentZone:
                    if (other.transform.TryGetComponent(out IPlayerPaymentZone currentPaymentZone))
                    {
                        OnTriggerPaymentZone?.Invoke(currentPaymentZone, false);                        
                    }
                    break;
            }
        }
        
        private void EnterTradeZone(ITradeZone tradeZone)
        {
            if (_tradeZones.Count == 0 || !ReferenceEquals(_tradeZones.Peek(), tradeZone))
            {
                _tradeZones.Enqueue(tradeZone);
                StartWaitingForTrigger(tradeZone);
            }
        }

        private void ExitTradeZone(ITradeZone tradeZone)
        {
            if (_tradeZones.Count > 0 && ReferenceEquals(_tradeZones.Peek(), tradeZone))
            {
                NotifyTradeZoneExit();
                _tradeZones.Dequeue();
                UpdateInteractionTrade();
            }
        }

        private void StartWaitingForTrigger(ITradeZone tradeZone)
        {
            _tradeZone = tradeZone;
            _isWaitingForTrigger = true;
            _elapsedTime = 0f;
        }

        private void CompleteWaitForTrigger()
        {
            _isWaitingForTrigger = false;
            _elapsedTime = 0f;
            
            if (_tradeZone != null)
            {
                Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {_tradeZone}");
                OnTriggerTradeZone?.Invoke(_tradeZone, true);
            }
            else
            {
                Debug.LogWarning("[CompleteWaitForTrigger] InteractionTrade component is null or _interactionTrade is null.");
            }
        }

        private void UpdateInteractionTrade()
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
            _tradeZone = null;
            _isWaitingForTrigger = false;
            _elapsedTime = 0f;
        }

        private void NotifyTradeZoneExit()
        {
            ITradeZone tradeZoneZone = _tradeZones.Peek();
            
            if (tradeZoneZone != null)
            {
                Debug.Log($"[NotifyTradeZoneExit] Exiting trade zone: {tradeZoneZone}");
                OnTriggerTradeZone?.Invoke(tradeZoneZone, false);
            }
        }
    }
}