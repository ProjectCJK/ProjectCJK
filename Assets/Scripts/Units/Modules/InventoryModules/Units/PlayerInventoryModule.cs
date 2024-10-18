using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.StatsModules.Units;
using Units.Stages.Buildings.Modules;
using Units.Stages.Controllers;
using Units.Stages.Creatures.Enums;
using Units.Stages.Items.Enums;
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
            if (!IsReadyToSend()) return;
            
            // 연결된 InteractionZone이 없거나, 인벤토리에 InteractionZone의 InputItemKey가 존재하지 않을 경우
            if (_targetInteractionZone == null) return;

            foreach (Tuple<EMaterialType, EProductType> inputItemKey in _targetInteractionZone.InputItemKey)
            {
                if (Inventory.TryGetValue(inputItemKey, out var OutputItemCount))
                {
                    // InteractionZone이 아이템을 받을 수 있는 상황이고, 인벤토리에 InputItemKey의 Value가 0 초과라면
                    if (_targetInteractionZone.CanReceiveItem() && OutputItemCount > 0)
                    {
                        RemoveItem(inputItemKey);
                        _targetInteractionZone.ReceiveItem(inputItemKey);
                        _itemController.TransferItem(inputItemKey, _senderTransform.position, _targetInteractionZone.ReceiverTransform);
                    }   
                }
            }
            
            SetLastSendTime();
        }

        public void ConnectWithInteractionTradeZone(Transform interactionZone, bool isConnected)
        {
            if (isConnected)
            {
                _targetInteractionZone = interactionZone.GetComponent<IInteractionTrade>();
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