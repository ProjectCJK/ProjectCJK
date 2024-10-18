using System;
using System.Collections.Generic;
using Units.Modules.InventoryModules.Abstract;
using Units.Stages.Controllers;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units
{
    public interface IFoodStandInventoryModule : IBuildingInventoryModule
    {
        
    }
    
    public class FoodStandInventoryModule : BuildingInventoryModule, IFoodStandInventoryModule
    {
        public FoodStandInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemController itemController,
            List<Tuple<EMaterialType, EProductType>> inputItemKey,
            List<Tuple<EMaterialType, EProductType>> outputItemKey)
            : base(senderTransform, receiverTransform, inventoryProperty, itemController, inputItemKey, outputItemKey)
        {
        }
    }
}