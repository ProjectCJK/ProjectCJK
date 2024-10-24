using System;
using System.Collections.Generic;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IKitchenInventoryModule : IBuildingInventoryModule
    {
    }
    
    public class KitchenInventoryModule : BuildingInventoryModule, IKitchenInventoryModule
    {
        public KitchenInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemController itemController,
            List<Tuple<EMaterialType, EItemType>> inputItemKey,
            List<Tuple<EMaterialType, EItemType>> outputItemKey)
            : base(senderTransform, receiverTransform, itemController, inventoryProperty, inputItemKey, outputItemKey)
        {
        }

        public void HandleOnItemReceivedFromProductModule(Tuple<EMaterialType, EItemType> itemKey, IItem item)
        {
            AddItem(itemKey);
            PushSpawnedItem(item);
        }
    }
}