using System;
using System.Collections.Generic;
using Units.Modules.CollisionModules.Abstract;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Modules.CollisionModules.Units
{
    public interface IGuestCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Func<string, bool> OnCompareWithTarget;
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
    }

    public class GuestCollisionModule : CollisionModule, IGuestCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Func<string, bool> OnCompareWithTarget;
        
        private readonly ECreatureType _creatureType;
        private readonly float _waitingTime;

        private IInteractionTrade _tradeZone;
        
        public GuestCollisionModule(IGuestStatModule guestStatModule)
        {
            _creatureType = guestStatModule.Type;
            _waitingTime = guestStatModule.WaitingTime;    
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
                        if (OnCompareWithTarget != null && OnCompareWithTarget(currentInteractionTrade.BuildingKey))
                        {
                            _tradeZone = currentInteractionTrade;
                            OnTriggerTradeZone?.Invoke(currentInteractionTrade, true);
                            
                            Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {_tradeZone}");
                        }   
                    }
                    break;
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            
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
                        if (_tradeZone != null && _tradeZone == currentInteractionTrade)
                        {
                            _tradeZone = null;
                            OnTriggerTradeZone?.Invoke(currentInteractionTrade, false);
                            
                            Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {_tradeZone}");
                        }   
                    }
                    break;
            }
        }
    }
}