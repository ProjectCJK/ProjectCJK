using System;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IStandInventoryModule : IBuildingInventoryModule
    {
        public event Action<int> OnMoneyReceived;
    }

    public class StandInventoryModule : BuildingInventoryModule, IStandInventoryModule
    {
        public StandInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            StandStatsModule standStatsModule,
            IItemFactory itemFactory,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, standStatsModule, inputItemKey, outputItemKey)
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
            else
            {
                AddItem(inputItemKey, item.Count);
                PushSpawnedItem(ReceiverTransform, item);
            }
        }
    }
}