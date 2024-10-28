using System;
using System.Collections.Generic;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Buildings;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IKitchenProductInventoryModule : IBuildingInventoryModule
    {
    }
    
    public class KitchenProductInventoryModule : BuildingInventoryModule, IKitchenProductInventoryModule
    {
        public KitchenProductInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IKitchenStatsModule kitchenStatsModule,
            IItemController itemController,
            Tuple<EMaterialType, EItemType> inputItemKey,
            Tuple<EMaterialType, EItemType> outputItemKey)
            : base(senderTransform, receiverTransform, itemController, kitchenStatsModule, inputItemKey, outputItemKey)
        {
        }

        protected override void OnItemReceived(Tuple<EMaterialType, EItemType> inputItemKey, IItem item)
        {
            AddItem(inputItemKey);
            PushSpawnedItem(item);
        }
    }
}