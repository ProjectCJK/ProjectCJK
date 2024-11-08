using System;
using Units.Stages.Modules.FactoryModules.Units;
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
        public event Action<int> OnMoneyReceived;
        
        public WareHouseInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            BuildingStatsModule buildingStatsModule,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, buildingStatsModule, inputItemKey, outputItemKey)
        {
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