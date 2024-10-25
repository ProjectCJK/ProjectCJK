using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
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
        public override IItemController ItemController { get; }
        public override int MaxInventorySize => _inventoryProperty.MaxInventorySize;

        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        private readonly IInventoryProperty _inventoryProperty;
        private readonly IItemController _itemController;

        private IInteractionTrade _targetInteractionZone;
        private IPlayerInventoryModule _playerInventoryModuleImplementation;

        public PlayerInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemController itemController,
            ECreatureType creatureType)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            _inventoryProperty = inventoryProperty;
            ItemController = itemController;
            CreatureType = creatureType;
        }

        public override void Initialize()
        {
            Inventory.Clear();
        }
        
        protected override void SendItem()
        {
            if (!IsReadyToSend()) return;
            
            if (_targetInteractionZone == null) return;

            var targetInputItemKey = _targetInteractionZone.InputItemKey;
            
            if (Inventory.TryGetValue(targetInputItemKey, out var OutputItemCount) && OutputItemCount > 0)
            {
                if (_targetInteractionZone.ReceiveItemWithDestroy(targetInputItemKey, SenderTransform.position))
                {
                    RemoveItem(targetInputItemKey);
                }
            }
            
            SetLastSendTime();
        }

        public void ConnectWithInteractionTradeZone(IInteractionTrade interactionZone, bool isConnected)
        {
            if (isConnected)
            {
                _targetInteractionZone = interactionZone;
                _targetInteractionZone.RegisterItemReceiver(this);
            }
            else if (_targetInteractionZone != null)
            {
                _targetInteractionZone.UnregisterItemReceiver(this);
                _targetInteractionZone = null;
            }
        }
    }
}