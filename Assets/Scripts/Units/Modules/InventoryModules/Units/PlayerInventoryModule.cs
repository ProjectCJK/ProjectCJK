using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Games.Buildings.Abstract;
using Units.Games.Buildings.Modules;
using Units.Games.Creatures.Enums;
using Units.Games.Items.Controllers;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.StatsModules.Units;
using UnityEngine;
using IItemReceiver = Units.Modules.InventoryModules.Interfaces.IItemReceiver;

namespace Units.Modules.InventoryModules.Units
{
    public interface IPlayerInventoryModule : IInventoryModule, ICreatureItemReceiver
    {
        
    }

    public class PlayerInventoryModule : InventoryModule, IPlayerInventoryModule
    {
        public ECreatureType CreatureType { get; private set; }
        public override int MaxInventorySize => _inventoryProperty.MaxInventorySize;
        public override Transform ReceiverTransform { get; }

        private readonly IInventoryProperty _inventoryProperty;
        private readonly IItemController _itemController;
        private readonly Transform _senderTransform;
        
        private IInteractionTrade _targetInteractionZone;
        private IPlayerInventoryModule _playerInventoryModuleImplementation;

        public PlayerInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemController itemController,
            ECreatureType creatureType)
        {
            _senderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            _inventoryProperty = inventoryProperty;
            _itemController = itemController;
            CreatureType = creatureType;
        }

        public override void Initialize()
        {
            Inventory.Clear();
        }

        public override void SendItem()
        {
            // 연결된 InteractionZone이 없거나, 인벤토리에 InteractionZone의 InputItemKey가 존재하지 않을 경우
            if (_targetInteractionZone == null) return;
            if (!Inventory.TryGetValue(_targetInteractionZone.InputItemKey, out var OutputItemCount)) return;
            
            // InteractionZone이 아이템을 받을 수 있는 상황이고, 인벤토리에 InputItemKey의 Value가 0 초과라면
            if (_targetInteractionZone.CanReceiveItem() && OutputItemCount > 0)
            {
                RemoveItem(_targetInteractionZone.InputItemKey);
                _targetInteractionZone.ReceiveItem(_targetInteractionZone.InputItemKey);
                _itemController.TransferItem(_targetInteractionZone.InputItemKey, _senderTransform.position, _targetInteractionZone.ReceiverTransform);
            }
        }

        public void ConnectWithInteractionTradeZone(Transform interactionZone, bool isConnected)
        {
            if (isConnected)
            {
                _targetInteractionZone = interactionZone.GetComponent<IInteractionTrade>();
                _targetInteractionZone.RegisterItemReceiver(this);
            }
            else
            {
                _targetInteractionZone.UnregisterItemReceiver(this);
                _targetInteractionZone = null;
            }
        }
    }
}