using System;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.HuntingZoneInventoryModules
{
    public interface IHuntingZoneInventoryModule : IBuildingInventoryModule
    {
        public event Action<int> OnMoneyReceived;
    }

    public class HuntingZoneInventoryModule : BuildingInventoryModule, IHuntingZoneInventoryModule
    {
        public HuntingZoneInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            BuildingStatsModule buildingStatsModule,
            string inputItemKey,
            string outputItemKey) : base(senderTransform, receiverTransform, itemFactory, buildingStatsModule,
            inputItemKey, outputItemKey)
        {
        }

        public event Action<int> OnMoneyReceived;

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            if (Enum.TryParse(inputItemKey, out ECurrencyType currencyType))
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