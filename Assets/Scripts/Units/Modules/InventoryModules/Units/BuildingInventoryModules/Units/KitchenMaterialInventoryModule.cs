using System;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Modules.ProductModules;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Buildings;
using Units.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IKitchenMaterialInventoryModule : IBuildingInventoryModule
    {
        
    }
    
    public class KitchenMaterialInventoryModule : BuildingInventoryModule, IKitchenMaterialInventoryModule
    {
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
            AddItem(inputItemKey);
            ItemFactory.ReturnItem(item);
        }
    }
}