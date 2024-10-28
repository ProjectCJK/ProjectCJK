using System;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Modules.ProductModules;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Buildings;
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
            IItemController itemController,
            Tuple<EMaterialType, EItemType> inputItemKey,
            Tuple<EMaterialType, EItemType> outputItemKey)
            : base(senderTransform, receiverTransform, itemController, inventoryProperty, inputItemKey, outputItemKey)
        {
        }

        protected override void OnItemReceived(Tuple<EMaterialType, EItemType> inputItemKey, IItem item)
        {
            AddItem(inputItemKey);
            ItemController.ReturnItem(item);
        }
    }
}