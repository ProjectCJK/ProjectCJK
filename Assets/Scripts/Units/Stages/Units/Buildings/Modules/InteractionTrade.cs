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
    public interface IInteractionTrade : IRegisterReference<Transform, IBuildingInventoryModule, Tuple<EMaterialType, EItemType>>, IItemReceiver
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
        
        private IBuildingInventoryModule _buildingInventoryModule;

        public void RegisterReference(Transform receiverTransform, IBuildingInventoryModule buildingInventoryModule, Tuple<EMaterialType, EItemType> inputItemKey)
        {
            ReceiverTransform = receiverTransform;
            _buildingInventoryModule = buildingInventoryModule;
            InputItemKey = inputItemKey;
        }

        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            _buildingInventoryModule.RegisterItemReceiver(itemReceiver);
        }

        public void UnregisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            _buildingInventoryModule.UnRegisterItemReceiver(itemReceiver);
        }

        public bool ReceiveItemWithDestroy(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition)
        {
            return _buildingInventoryModule.ReceiveItemWithDestroy(inputItemKey, currentSenderPosition, ReceiverTransform.transform.position);
        }

        public bool ReceiveItemWithDestroy(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition, Vector3 targetReceiverPosition)
        {
            return _buildingInventoryModule.ReceiveItemWithDestroy(inputItemKey, currentSenderPosition, ReceiverTransform.transform.position);
        }

        public bool ReceiveItemWithoutDestroy(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition)
        {
            return _buildingInventoryModule.ReceiveItemWithoutDestroy(inputItemKey, currentSenderPosition, ReceiverTransform.transform.position);
        }

        public bool ReceiveItemWithoutDestroy(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition, Vector3 targetReceiverPosition)
        {
            return _buildingInventoryModule.ReceiveItemWithoutDestroy(inputItemKey, currentSenderPosition, ReceiverTransform.transform.position);
        }

        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey) => _buildingInventoryModule.HasMatchingItem(InventoryKey);

        public bool CanReceiveItem() => _buildingInventoryModule.CanReceiveItem();
    }
}
