using System;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IDeliveryLodgingInventoryModule : IBuildingInventoryModule
    {
        public event Action<int> OnMoneyReceived;
    }

    public class DeliveryLodgingInventoryModule : BuildingInventoryModule, IDeliveryLodgingInventoryModule
    {
        public DeliveryLodgingInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            DeliveryLodgingStatsModule deliveryLodgingStatsModule,
            string inputItemKey, string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, deliveryLodgingStatsModule, inputItemKey,
                outputItemKey)
        {
        }

        public event Action<int> OnMoneyReceived;

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
        }
    }
}