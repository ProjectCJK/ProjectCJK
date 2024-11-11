using Interfaces;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Units.Buildings.Modules.UnlockZones.Abstract
{
    public interface IUnlockZone :
        IRegisterReference<Transform, IBuildingInventoryModule, IBuildingInventoryModule, string, string>, IItemReceiver
    {
        public string BuildingKey { get; }
        public string InputItemKey { get; }
        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register);
    }

    [RequireComponent(typeof(TilemapCollider2D))]
    public abstract class UnlockZone : MonoBehaviour, IUnlockZone
    {
        private IBuildingInventoryModule _buildingReceiverInventoryModule;
        private IBuildingInventoryModule _buildingSenderInventoryModule;
        public Transform SenderTransform => null;
        public Transform ReceiverTransform { get; private set; }

        public string BuildingKey { get; private set; }
        public string InputItemKey { get; private set; }

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

        public void ReceiveItemThroughTransfer(string inputItemKey, int count, Vector3 currentSenderPosition)
        {
            _buildingReceiverInventoryModule.ReceiveItemThroughTransfer(inputItemKey, count, currentSenderPosition);
        }

        public bool HasMatchingItem(string InventoryKey)
        {
            return _buildingReceiverInventoryModule.HasMatchingItem(InventoryKey);
        }

        public bool CanReceiveItem()
        {
            return _buildingReceiverInventoryModule.CanReceiveItem();
        }
    }
}