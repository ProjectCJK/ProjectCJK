using System;
using System.Collections.Generic;
using Units.Modules.CollisionModules.Abstract;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Modules.CollisionModules.Units
{
    public interface IGuestCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Func<string, bool> OnCompareWithTarget;
        public event Action OnTriggerSpawnZone;
        
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
    }

    public class GuestCollisionModule : CollisionModule, IGuestCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Func<string, bool> OnCompareWithTarget;
        public event Action OnTriggerSpawnZone;

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
                    var currentTradeZone = other.transform.GetComponent<IInteractionTrade>();

                    if (currentTradeZone.CheckAccessor(_creatureType))
                    {
                        if (OnCompareWithTarget != null && OnCompareWithTarget(currentTradeZone.BuildingKey))
                        {
                            _tradeZone = currentTradeZone;
                            OnTriggerTradeZone?.Invoke(currentTradeZone, true);
                            
                            Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {_tradeZone}");
                        }   
                    }
                    break;
                case ECollisionType.SpawnZone:
                    var currentSpawnZone = other.transform.GetComponent<ISpawnZone>();

                    if (currentSpawnZone.CreatureType == _creatureType)
                    {
                        if (currentSpawnZone is IGuestSpawnZone { NpcType: ENPCType.Guest })
                        {
                            OnTriggerSpawnZone?.Invoke();
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