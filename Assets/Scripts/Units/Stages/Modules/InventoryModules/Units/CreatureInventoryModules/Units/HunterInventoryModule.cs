using System.Collections.Generic;
using Managers;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Units
{
    public interface IHunterInventoryModule : ICreatureInventoryModule
    {
    }

    public class HunterInventoryModule : CreatureInventoryModule, IHunterInventoryModule
    {
        protected override Dictionary<string, int> Inventory { get; set; } = new ();
        
        private readonly ENPCType _npcType;

        public HunterInventoryModule(
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

        public override ECreatureType CreatureType { get; }

        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        public override void RegisterItemReceiver(ITradeZone zone, bool isConnected)
        {
            if (isConnected)
                RegisterZone(zone as INPCTradeZone);
            else
                UnregisterZone(zone as INPCTradeZone);
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey, item.Count);
            ItemFactory.ReturnItem(item);
        }

        private void RegisterZone(INPCTradeZone zone)
        {
            if (zone.CheckInputAccessorNPC(_npcType) && interactionTradeZones.Add(zone))
                Debug.Log($"{_npcType} => {zone.BuildingKey} 도킹 완료 @~@");

            if (zone.CheckOutputAccessorNPC(_npcType) && zone.RegisterItemReceiver(this, true))
                Debug.Log($"{zone.BuildingKey} => {_npcType} 도킹 완료 @~@");
        }

        private void UnregisterZone(INPCTradeZone zone)
        {
            if (zone.CheckInputAccessorNPC(_npcType) && interactionTradeZones.Remove(zone))
                Debug.Log($"{_npcType} => {zone.BuildingKey} 도킹 해제 완료 @~@");

            if (zone.CheckOutputAccessorNPC(_npcType) && zone.RegisterItemReceiver(this, false))
                Debug.Log($"{zone.BuildingKey} => {_npcType} 도킹 해제 완료 @~@");
        }
    }
}