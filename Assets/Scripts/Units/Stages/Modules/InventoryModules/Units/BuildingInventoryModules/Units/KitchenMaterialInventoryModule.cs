using System;
using Managers;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IKitchenMaterialInventoryModule : IBuildingInventoryModule
    {
        public event Action<int> OnMoneyReceived;
    }
    
    public class KitchenMaterialInventoryModule : BuildingInventoryModule, IKitchenMaterialInventoryModule
    {
        public event Action<int> OnMoneyReceived;
        
        public KitchenMaterialInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IKitchenStatsModule inventoryProperty,
            IItemFactory itemFactory,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, inventoryProperty, inputItemKey, outputItemKey)
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
            }
            else
            {
                AddItem(inputItemKey);
            }
            
            ItemFactory.ReturnItem(item);
        }
    }
}