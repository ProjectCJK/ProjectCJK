using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.StatsModules.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;
using IItemReceiver = Units.Modules.InventoryModules.Interfaces.IItemReceiver;

namespace Units.Modules.InventoryModules.Units
{
    public interface IPlayerInventoryModule : IInventoryModule, ICreatureItemReceiver
    {
    }

    public class PlayerInventoryModule : InventoryModule, IPlayerInventoryModule
    {
        public ECreatureType CreatureType { get; }
        public override IItemFactory ItemFactory { get; }
        public override int MaxInventorySize => _inventoryProperty.MaxProductInventorySize;

        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }
        
        private readonly HashSet<IInteractionTrade> _interactionTradeZones = new();
        private readonly IInventoryProperty _inventoryProperty;

        private IPlayerInventoryModule _playerInventoryModuleImplementation;

        public PlayerInventoryModule(
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
            if (!IsReadyToSend() || _interactionTradeZones.Count <= 0) return;
            
            foreach (IInteractionTrade targetInteractionZone in _interactionTradeZones.ToList())
            {
                ProcessInteractionZone(targetInteractionZone);
            }

            SetLastSendTime();
        }

        private void ProcessInteractionZone(IInteractionTrade targetInteractionZone)
        {
            if (targetInteractionZone == null) return;

            Tuple<EMaterialType, EItemType> targetInputItemKey = targetInteractionZone.InputItemKey;

            if (Inventory.TryGetValue(targetInputItemKey, out var outputItemCount) && outputItemCount > 0)
            {
                if (targetInteractionZone.ReceiveItem(targetInputItemKey, SenderTransform.position))
                {
                    RemoveItem(targetInputItemKey);
                }
            }
        }

        protected override void OnItemReceived(Tuple<EMaterialType, EItemType> inputItemKey, IItem item)
        {
            AddItem(inputItemKey);
            ItemFactory.ReturnItem(item);
        }

        public void ConnectWithInteractionTradeZone(IInteractionTrade interactionZone, bool isConnected)
        {
            if (isConnected)
            {
                if (_interactionTradeZones.Add(interactionZone))
                {
                    interactionZone.RegisterItemReceiver(this);
                }
            }
            else
            {
                if (_interactionTradeZones.Remove(interactionZone))
                {
                    interactionZone.UnregisterItemReceiver(this);
                }
            }
        }
    }
}