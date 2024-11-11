using System;
using Units.Stages.Modules.CollisionModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Buildings.Modules.TradeZones.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.HuntingZones;
using UnityEngine;

namespace Units.Stages.Modules.CollisionModules.Units
{
    public interface IHunterCollisionModule
    {
        public event Action<INPCTradeZone, bool> OnTriggerTradeZone;
        public event Action<bool> OnTriggerHuntingZone;

        public event Func<string, bool> OnCompareWithTarget;

        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
    }

    public class HunterCollisionModule : CollisionModule, IHunterCollisionModule
    {
        private readonly ECreatureType _creatureType;
        private readonly ENPCType _npcType;

        private INPCTradeZone _tradeZoneZone;

        public HunterCollisionModule(IHunterStatsModule _deliveryManStatModule)
        {
            _creatureType = _deliveryManStatModule.CreatureType;
            _npcType = _deliveryManStatModule.NPCType;
        }

        public event Action<INPCTradeZone, bool> OnTriggerTradeZone;
        public event Func<string, bool> OnCompareWithTarget;
        public event Action<bool> OnTriggerHuntingZone;

        public void OnTriggerEnter2D(Collider2D other)
        {
            switch (CheckLayer(other.gameObject.layer))
            {
                case ECollisionType.None:
                    break;

                case ECollisionType.TradeZone:
                    if (other.transform.TryGetComponent(out INPCTradeZone currentTradeZone))
                        if (currentTradeZone.CheckAccessorNPC(_npcType))
                            if (OnCompareWithTarget != null && OnCompareWithTarget(currentTradeZone.BuildingKey))
                            {
                                _tradeZoneZone = currentTradeZone;
                                OnTriggerTradeZone?.Invoke(currentTradeZone, true);

                                Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {_tradeZoneZone}");
                            }

                    break;

                case ECollisionType.HuntingZone:
                    var currentHuntingZone = other.transform.GetComponentInParent<IHuntingZone>();
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
                    if (other.transform.TryGetComponent(out INPCTradeZone currentTradeZone))
                        if (currentTradeZone.CheckAccessorNPC(_npcType))
                            if (_tradeZoneZone != null && _tradeZoneZone == currentTradeZone)
                            {
                                _tradeZoneZone = null;
                                OnTriggerTradeZone?.Invoke(currentTradeZone, false);

                                Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {_tradeZoneZone}");
                            }

                    break;

                case ECollisionType.HuntingZone:
                    var currentHuntingZone = other.transform.GetComponentInParent<IHuntingZone>();
                    if (currentHuntingZone != null) OnTriggerHuntingZone?.Invoke(false);
                    break;
            }
        }
    }
}