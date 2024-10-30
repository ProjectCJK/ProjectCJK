using System;
using System.Collections.Generic;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.CreatureInventoryModules.Units
{
    public interface IGuestInventoryModule : ICreatureInventoryModule, ICreatureItemReceiver
    {
        
    }
    
    public class GuestInventoryModule : CreatureInventoryModule, IGuestInventoryModule
    {
        public ECreatureType CreatureType { get; }
        public override IItemFactory ItemFactory { get; }
        public override int MaxInventorySize => _inventoryProperty.MaxProductInventorySize;
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }
        
        private readonly HashSet<IInteractionTrade> _interactionTradeZones = new();
        private readonly IInventoryProperty _inventoryProperty;

        public GuestInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemFactory itemController,
            ECreatureType creatureType)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            _inventoryProperty = inventoryProperty;
            ItemFactory = itemController;
            CreatureType = creatureType;
        }
        
        public override void Initialize()
        {
            Inventory.Clear();
        }

        protected override void SendItem()
        {
            
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey);
            PushSpawnedItem(item);
        }
        
        public void ConnectWithInteractionTradeZone(IInteractionTrade interactionZone, bool isConnected)
        {
            switch (isConnected)
            {
                case true:
                    if (_interactionTradeZones.Add(interactionZone)) interactionZone.RegisterItemReceiver(this, true);
                    break;
                case false:
                    if (_interactionTradeZones.Remove(interactionZone))  interactionZone.RegisterItemReceiver(this, false);
                    break;
            }
        }
    }
}