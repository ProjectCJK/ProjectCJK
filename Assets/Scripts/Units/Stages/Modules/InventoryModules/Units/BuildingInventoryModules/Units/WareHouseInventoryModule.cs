using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
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
        public override Dictionary<string, int> Inventory
        {
            get
            {
                if (!GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey(_upgradableBuildingStatsModule.BuildingKey))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems.TryAdd(_upgradableBuildingStatsModule.BuildingKey, new Dictionary<string, int>());
                }
                
                return GameManager.Instance.ES3Saver.BuildingOutputItems[_upgradableBuildingStatsModule.BuildingKey];
            }
            set
            {
                if (!GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey(_upgradableBuildingStatsModule.BuildingKey))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems.TryAdd(_upgradableBuildingStatsModule.BuildingKey, new Dictionary<string, int>());
                }

                if (!GameManager.Instance.ES3Saver.BuildingOutputItems[_upgradableBuildingStatsModule.BuildingKey].ContainsKey(value.Keys.ToString()))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems[_upgradableBuildingStatsModule.BuildingKey].TryAdd(_upgradableBuildingStatsModule.BuildingKey, int.Parse(value.Values.ToString()));
                }
            }
        }
        
        private readonly UpgradableBuildingStatsModule _upgradableBuildingStatsModule;
        
        public WareHouseInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            UpgradableBuildingStatsModule upgradableBuildingStatsModule,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, upgradableBuildingStatsModule, inputItemKey, outputItemKey)
        {
            _upgradableBuildingStatsModule = upgradableBuildingStatsModule;
        }

        public event Action<int> OnMoneyReceived;

        public void SendItem(ICreatureItemReceiver itemReceiver, EMaterialType materialType)
        {
            if (!IsReadyToSend()) return;

            var outputItemKey = $"{EItemType.Material}_{materialType}";

            if (!Inventory.TryGetValue(outputItemKey, out var itemCount) || itemCount <= 0) return;

            if (!itemReceiver.CanReceiveItem()) return;

            itemReceiver.ReceiveItemThroughTransfer(outputItemKey, 1, SenderTransform.position);
            RemoveItem(outputItemKey);
            OnUpdateStackedItem?.Invoke(CurrentInventorySize);

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
                ItemFactory.ReturnItem(item);
                OnUpdateStackedItem?.Invoke(Inventory[inputItemKey]);
            }
        }
    }
}