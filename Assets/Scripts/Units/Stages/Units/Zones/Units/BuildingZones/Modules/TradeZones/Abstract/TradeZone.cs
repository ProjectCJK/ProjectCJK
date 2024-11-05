using Interfaces;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Units.Zones.Units.BuildingZones.Abstract;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract
{
    public interface ITradeZone : IRegisterReference<IUnlockZoneProperty, Transform, IBuildingInventoryModule, IBuildingInventoryModule, string, string>, IItemReceiver
    {
        public string BuildingKey { get; }
        public string InputItemKey { get; }
        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register);
        
        public bool CanReceiveMoney();
    }
    
    [RequireComponent(typeof(TilemapCollider2D))]
    public class TradeZone : MonoBehaviour, ITradeZone
    {
        public Transform SenderTransform => null;
        public Transform ReceiverTransform { get; private set; }

        public string BuildingKey { get; private set; }
        public string InputItemKey { get; private set; }

        private IUnlockZoneProperty _building;
        private IBuildingInventoryModule _buildingReceiverInventoryModule;
        private IBuildingInventoryModule _buildingSenderInventoryModule;
        
        public void RegisterReference(
            IUnlockZoneProperty buildingZone,
            Transform receiverTransform,
            IBuildingInventoryModule buildingReceiverInventoryModule,
            IBuildingInventoryModule buildingSenderInventoryModule,
            string buildingKey,
            string inputItemKey)
        {
            _building = buildingZone;
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

        public bool CanReceiveMoney()
        {
            if (_building is IUnlockZoneProperty unlockZoneProperty)
            {
                return unlockZoneProperty.CurrentGoldForUnlock < unlockZoneProperty.RequiredGoldForUnlock;
            }

            return false;
        }

        public void ReceiveItemThroughTransfer(string inputItemKey, int count, Vector3 currentSenderPosition)
        {
            _buildingReceiverInventoryModule.ReceiveItemThroughTransfer(inputItemKey, count, currentSenderPosition);
        }

        public bool HasMatchingItem(string InventoryKey) => _buildingReceiverInventoryModule.HasMatchingItem(InventoryKey);

        public bool CanReceiveItem() => _buildingReceiverInventoryModule.CanReceiveItem();
    }
}
