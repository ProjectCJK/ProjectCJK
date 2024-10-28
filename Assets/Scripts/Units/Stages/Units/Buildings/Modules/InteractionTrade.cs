using System;
using System.Collections.Generic;
using Interfaces;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Units.Buildings.Modules
{
    public interface IInteractionTrade : IRegisterReference<Transform, IBuildingInventoryModule, IBuildingInventoryModule, Tuple<EMaterialType, EItemType>>, IItemReceiver
    {
        public Tuple<EMaterialType, EItemType> InputItemKey { get; }
        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver);
        public void UnregisterItemReceiver(ICreatureItemReceiver itemReceiver);
    }
    
    [RequireComponent(typeof(TilemapCollider2D))]
    public class InteractionTrade : MonoBehaviour, IInteractionTrade
    {
        public Transform SenderTransform { get; }
        public Transform ReceiverTransform { get; private set; }
        
        public Tuple<EMaterialType, EItemType> InputItemKey { get; private set; }
        
        private IBuildingInventoryModule _buildingReceiverInventoryModule;
        private IBuildingInventoryModule _buildingSenderInventoryModule;

        public void RegisterReference(Transform receiverTransform, IBuildingInventoryModule buildingReceiverInventoryModule, IBuildingInventoryModule buildingSenderInventoryModule, Tuple<EMaterialType, EItemType> inputItemKey)
        {
            ReceiverTransform = receiverTransform;
            _buildingReceiverInventoryModule = buildingReceiverInventoryModule;
            _buildingSenderInventoryModule = buildingSenderInventoryModule;
            InputItemKey = inputItemKey;
        }

        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            _buildingSenderInventoryModule.RegisterItemReceiver(itemReceiver);
        }

        public void UnregisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            _buildingSenderInventoryModule.UnRegisterItemReceiver(itemReceiver);
        }

        public bool ReceiveItem(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition)
        {
            return _buildingReceiverInventoryModule.ReceiveItem(inputItemKey, currentSenderPosition);
        }

        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey) => _buildingReceiverInventoryModule.HasMatchingItem(InventoryKey);

        public bool CanReceiveItem() => _buildingReceiverInventoryModule.CanReceiveItem();
    }
}
