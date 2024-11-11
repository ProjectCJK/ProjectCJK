using System;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IWareHouseInventoryModule : IBuildingInventoryModule
    {
        public event Action<int> OnMoneyReceived;
    }

    public class WareHouseInventoryModule : BuildingInventoryModule, IWareHouseInventoryModule
    {
        public WareHouseInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            UpgradableBuildingStatsModule upgradableBuildingStatsModule,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, upgradableBuildingStatsModule, inputItemKey, outputItemKey)
        {
        }

        public event Action<int> OnMoneyReceived;

        public void SendItem(ICreatureItemReceiver itemReceiver, EMaterialType materialType)
        {
            if (!IsReadyToSend()) return;

            var outputItemKey = $"{EItemType.Material}_{materialType}";

            if (!Inventory.TryGetValue(outputItemKey, out var itemCount) || itemCount <= 0) return;

            if (!itemReceiver.CanReceiveItem()) return;

            IItem item = PopSpawnedItem();
            if (item != null)
            {
                itemReceiver.ReceiveItemThroughTransfer(outputItemKey, item.Count, item.Transform.position);
                ItemFactory.ReturnItem(item);
                RemoveItem(outputItemKey);
            }

            SetLastSendTime();
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            if (Enum.TryParse(inputItemKey, out ECurrencyType currencyType))
            {
                switch (currencyType)
                {
                    case ECurrencyType.Money:
                        OnMoneyReceived?.Invoke(item.Count);
                        break;
                }

                ItemFactory.ReturnItem(item);
            }
            else
            {
                AddItem(inputItemKey, item.Count);
                PushSpawnedItem(ReceiverTransform, item);
            }
        }
    }
}