using System;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Units;
using Unity.VisualScripting;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.CreatureInventoryModules.Units
{
    public interface IGuestInventoryModule : ICreatureInventoryModule
    {
        public event Action OnTargetQuantityReceived; 
    }

    public class GuestInventoryModule : CreatureInventoryModule, IGuestInventoryModule
    {
        public event Action OnTargetQuantityReceived; 
        public override ECreatureType CreatureType { get; }

        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        private readonly ENPCType _npcType;
        
        public GuestInventoryModule(
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
            AddItem(inputItemKey);
            PushSpawnedItem(ReceiverTransform, item);
            
            if (!CanReceiveItem()) OnTargetQuantityReceived?.Invoke();
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