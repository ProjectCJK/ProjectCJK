using System;
using System.Collections.Generic;
using Interfaces;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Units.Buildings.Modules
{
    public interface IInteractionTrade : IRegisterReference<Transform, IBuildingInventoryModule, IBuildingInventoryModule, string, string>, IItemReceiver
    {
        public string BuildingKey { get; }
        public string InputItemKey { get; }
        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register);
    }
    
    [RequireComponent(typeof(TilemapCollider2D))]
    public class InteractionTrade : MonoBehaviour, IInteractionTrade
    {
        public Transform SenderTransform { get; }
        public Transform ReceiverTransform { get; private set; }

        public string BuildingKey { get; private set; }
        public string InputItemKey { get; private set; }
        
        private IBuildingInventoryModule _buildingReceiverInventoryModule;
        private IBuildingInventoryModule _buildingSenderInventoryModule;
        
        public void RegisterReference(
            Transform receiverTransform,
            IBuildingInventoryModule buildingReceiverInventoryModule,
            IBuildingInventoryModule buildingSenderInventoryModule,
            string buildingKey,
            string inputItemKey)
        {
            ReceiverTransform = receiverTransform;
            _buildingReceiverInventoryModule = buildingReceiverInventoryModule;
            _buildingSenderInventoryModule = buildingSenderInventoryModule;
            BuildingKey = buildingKey;
            InputItemKey = inputItemKey;
        }

        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register)
        {
            _buildingSenderInventoryModule.RegisterItemReceiver(itemReceiver, register);
        }

        public bool ReceiveItem(string inputItemKey, Vector3 currentSenderPosition)
        {
            return _buildingReceiverInventoryModule.ReceiveItem(inputItemKey, currentSenderPosition);
        }

        public bool HasMatchingItem(string InventoryKey) => _buildingReceiverInventoryModule.HasMatchingItem(InventoryKey);

        public bool CanReceiveItem() => _buildingReceiverInventoryModule.CanReceiveItem();
    }
}
