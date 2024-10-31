using Interfaces;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Units.Buildings.Modules.TradeZones.Abstract
{
    public interface ITradeZone : IRegisterReference<Transform, IBuildingInventoryModule, IBuildingInventoryModule, string, string>, IItemReceiver
    {
        public string BuildingKey { get; }
        public string InputItemKey { get; }
        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register);
    }
    
    [RequireComponent(typeof(TilemapCollider2D))]
    public class TradeZone : MonoBehaviour, ITradeZone
    {
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
        
        public void ReceiveItemThroughTransfer(string inputItemKey, Vector3 currentSenderPosition)
        {
            _buildingReceiverInventoryModule.ReceiveItemThroughTransfer(inputItemKey, currentSenderPosition);
        }

        public bool HasMatchingItem(string InventoryKey) => _buildingReceiverInventoryModule.HasMatchingItem(InventoryKey);

        public bool CanReceiveItem() => _buildingReceiverInventoryModule.CanReceiveItem();
    }
}
