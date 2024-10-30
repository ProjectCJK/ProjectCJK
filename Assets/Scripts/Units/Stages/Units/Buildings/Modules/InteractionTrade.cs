using System;
using System.Collections.Generic;
using Interfaces;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Units.Buildings.Modules
{
    public interface IInteractionTrade : IRegisterReference<Transform, IBuildingInventoryModule, IBuildingInventoryModule, string, string>, IItemReceiver
    {
        public string BuildingKey { get; }
        public string InputItemKey { get; }
        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register);
        public bool CheckAccessor(ECreatureType creatureType);
        public bool CheckInputAccessor(ECreatureType creatureType);
        public bool CheckOutputAccessor(ECreatureType creatureType);
    }
    
    [RequireComponent(typeof(TilemapCollider2D))]
    public class InteractionTrade : MonoBehaviour, IInteractionTrade
    {
        [Header("접근 가능한 유닛 타입")]
        public List<ECreatureType> InputAccessCreatureTypes;
        public List<ECreatureType> OutAccessCreatureTypes;

        public Transform SenderTransform => null;
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

        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register)
        {
            return _buildingSenderInventoryModule.RegisterItemReceiver(itemReceiver, register);
        }

        public bool CheckAccessor(ECreatureType creatureType)
        {
            return InputAccessCreatureTypes.Contains(creatureType) || OutAccessCreatureTypes.Contains(creatureType);
        }

        public bool CheckOutputAccessor(ECreatureType creatureType)
        {
            return OutAccessCreatureTypes.Contains(creatureType);
        }

        public bool CheckInputAccessor(ECreatureType creatureType)
        {
            return InputAccessCreatureTypes.Contains(creatureType);
        }

        public void ReceiveItem(string inputItemKey, Vector3 currentSenderPosition)
        {
            _buildingReceiverInventoryModule.ReceiveItem(inputItemKey, currentSenderPosition);
        }

        public bool HasMatchingItem(string InventoryKey) => _buildingReceiverInventoryModule.HasMatchingItem(InventoryKey);

        public bool CanReceiveItem() => _buildingReceiverInventoryModule.CanReceiveItem();
    }
}
