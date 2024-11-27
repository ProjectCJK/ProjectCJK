using Interfaces;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.UnlockModules.Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Units.Buildings.Modules.TradeZones.Abstract
{
    public interface ITradeZone : IRegisterReference<IUnlockZoneProperty, Transform, IBuildingInventoryModule,
        IBuildingInventoryModule, string, string>, IItemReceiver
    {
        public string BuildingKey { get; }
        public string InputItemKey { get; }
        public int TempMoney { get; set; }
        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register);

        public int CanReceiveMoney();
    }

    [RequireComponent(typeof(TilemapCollider2D))]
    public class TradeZone : MonoBehaviour, ITradeZone
    {
        private IUnlockZoneProperty _building;
        private IBuildingInventoryModule _buildingReceiverInventoryModule;
        private IBuildingInventoryModule _buildingSenderInventoryModule;
        public Transform SenderTransform => null;
        public Transform ReceiverTransform { get; private set; }

        public string BuildingKey { get; private set; }
        public string InputItemKey { get; private set; }
        public int TempMoney
        {
            get => _buildingReceiverInventoryModule.TempMoney;
            set => _buildingReceiverInventoryModule.TempMoney = value;
        }

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

        public int CanReceiveMoney()
        {
            if (_building is { } unlockZoneProperty)
                return unlockZoneProperty.RequiredGoldForUnlock - (unlockZoneProperty.CurrentGoldForUnlock + TempMoney);

            return 0;
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