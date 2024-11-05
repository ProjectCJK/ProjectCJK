using System;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Units;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Units;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Creatures.Units
{
    public interface IDeliveryManInventoryModule : ICreatureInventoryModule
    {
        
    }
    
    public class DeliveryManInventoryModule : CreatureInventoryModule, IDeliveryManInventoryModule
    {
        public override ECreatureType CreatureType { get; }

        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        private readonly ENPCType _npcType;
        
        public DeliveryManInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemFactory itemFactory,
            ECreatureType creatureType,
            ENPCType npcType) : base(inventoryProperty)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            ItemFactory = itemFactory;
            CreatureType = creatureType;
            _npcType = npcType;
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey, item.Count);
            PushSpawnedItem(ReceiverTransform, item);
        }

        public override void RegisterItemReceiver(ITradeZone zone, bool isConnected)
        {
            if (isConnected)
            {
                RegisterZone(zone as INPCTradeZone);
            }
            else
            {
                UnregisterZone(zone as INPCTradeZone);
            }
        }

        private void RegisterZone(INPCTradeZone zone)
        {
            if (zone.CheckInputAccessorNPC(_npcType) && interactionTradeZones.Add(zone))
            {
                Debug.Log($"{CreatureType} => {zone.BuildingKey} 도킹 완료 @~@");
            }
            
            if (zone.CheckOutputAccessorNPC(_npcType) && zone.RegisterItemReceiver(this, true))
            {
                Debug.Log($"{zone.BuildingKey} => {CreatureType} 도킹 완료 @~@");
            }
        }

        private void UnregisterZone(INPCTradeZone zone)
        {
            if (zone.CheckInputAccessorNPC(_npcType) && interactionTradeZones.Remove(zone))
            {
                Debug.Log($"{CreatureType} => {zone.BuildingKey} 도킹 해제 완료 @~@");
            }
            
            if (zone.CheckOutputAccessorNPC(_npcType) && zone.RegisterItemReceiver(this, false))
            {
                Debug.Log($"{zone.BuildingKey} => {CreatureType} 도킹 해제 완료 @~@");
            }
        }
    }
}