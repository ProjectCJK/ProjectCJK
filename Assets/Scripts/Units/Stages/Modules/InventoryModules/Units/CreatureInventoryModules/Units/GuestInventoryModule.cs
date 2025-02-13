using System;
using System.Collections.Generic;
using System.Linq;
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
    public interface IGuestInventoryModule : ICreatureInventoryModule
    {
        public Tuple<string, int> GetItem();
        public event Action OnTargetQuantityReceived;
        public void Reset();
    }

    public class GuestInventoryModule : CreatureInventoryModule, IGuestInventoryModule
    {
        public override Dictionary<string, int> Inventory { get; set; } = new();

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

        public event Action OnTargetQuantityReceived;

        public override ECreatureType CreatureType { get; }

        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        public override void Initialize()
        {
            Inventory.Clear();
        }
        
        public override void RegisterItemReceiver(ITradeZone zone, bool isConnected)
        {
            if (isConnected)
                RegisterZone(zone as INPCTradeZone);
            else
                UnregisterZone(zone as INPCTradeZone);
        }

        public void Reset()
        {
            Inventory.Clear();

            IItem item = PopSpawnedItem();

            if (item != null) ItemFactory.ReturnItem(item);
        }

        public Tuple<string, int> GetItem()
        {
            List<string> returnItems = Inventory.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key).ToList();

            return returnItems.Count > 0 ? new Tuple<string, int>(returnItems[0], Inventory[returnItems[0]]) : null;
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey, item.Count);
            PushSpawnedItem(ReceiverTransform, item, true);

            if (!CanReceiveItem()) OnTargetQuantityReceived?.Invoke();
        }

        private void RegisterZone(INPCTradeZone zone)
        {
            if (zone.CheckInputAccessorNPC(_npcType) && interactionTradeZones.Add(zone))
            {
                
            }

            if (zone.CheckOutputAccessorNPC(_npcType) && zone.RegisterItemReceiver(this, true))
            {
                
            }
        }

        private void UnregisterZone(INPCTradeZone zone)
        {
            if (zone.CheckInputAccessorNPC(_npcType) && interactionTradeZones.Remove(zone))
            {
            }

            if (zone.CheckOutputAccessorNPC(_npcType) && zone.RegisterItemReceiver(this, false))
            {
                
            }
        }
    }
}