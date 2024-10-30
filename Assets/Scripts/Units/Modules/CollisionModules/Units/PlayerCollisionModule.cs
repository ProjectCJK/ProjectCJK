using System;
using System.Collections.Generic;
using Managers;
using Units.Modules.CollisionModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
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
        
        private readonly ECreatureType _creatureType;
        private readonly Queue<IInteractionTrade> _tradeZones = new();
        private readonly float _waitingTime;
        
        private IInteractionTrade _interactionTrade;
        private IInteractionTrade _previousInteractionTrade; // 이전 트리거 존을 저장
        
        private float _elapsedTime;  // 시간 측정 변수
        private bool _isWaitingForTrigger; // 대기 상태 여부

        public PlayerCollisionModule(IPlayerStatsModule playerStatsModule)
        {
            _creatureType = playerStatsModule.Type;
            _waitingTime = playerStatsModule.WaitingTime;
        }
        
        public void Update()
        {
            if (_isWaitingForTrigger && _interactionTrade != null)
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
                    var currentInteractionTrade = other.transform.GetComponent<IInteractionTrade>();
                    
                    if (currentInteractionTrade.CheckAccessor(_creatureType))
                    {
                        EnterTradeZone(currentInteractionTrade);   
                    }
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
                    var currentInteractionTrade = other.transform.GetComponent<IInteractionTrade>();
                    
                    if (currentInteractionTrade.CheckAccessor(_creatureType))
                    {
                        ExitTradeZone(currentInteractionTrade);   
                    }
                    break;

                case ECollisionType.UpgradeZone:
                    break;

                case ECollisionType.HuntingZone:
                    OnTriggerHuntingZone?.Invoke(false);
                    break;
            }
        }
        
        private void EnterTradeZone(IInteractionTrade interactionTrade)
        {
            if (_tradeZones.Count == 0 || !ReferenceEquals(_tradeZones.Peek(), interactionTrade))
            {
                _tradeZones.Enqueue(interactionTrade);
                StartWaitingForTrigger(interactionTrade);
            }
        }

        private void ExitTradeZone(IInteractionTrade interactionTrade)
        {
            if (_tradeZones.Count > 0 && ReferenceEquals(_tradeZones.Peek(), interactionTrade))
            {
                NotifyTradeZoneExit();
                _tradeZones.Dequeue();
                UpdateInteractionTrade();
            }
        }

        private void StartWaitingForTrigger(IInteractionTrade interactionTrade)
        {
            _interactionTrade = interactionTrade;
            _isWaitingForTrigger = true;
            _elapsedTime = 0f;
        }

        private void CompleteWaitForTrigger()
        {
            _isWaitingForTrigger = false;
            _elapsedTime = 0f;
            
            if (_interactionTrade != null)
            {
                Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {_interactionTrade}");
                OnTriggerTradeZone?.Invoke(_interactionTrade, true);
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
            _interactionTrade = null;
            _isWaitingForTrigger = false;
            _elapsedTime = 0f;
        }

        private void NotifyTradeZoneExit()
        {
            IInteractionTrade tradeZone = _tradeZones.Peek();
            
            if (tradeZone != null)
            {
                Debug.Log($"[NotifyTradeZoneExit] Exiting trade zone: {tradeZone}");
                OnTriggerTradeZone?.Invoke(tradeZone, false);
            }
        }
    }
}