using System;
using System.Collections.Generic;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Modules.ProductModules.Units
{
    public interface IKitchenProductModule : IBuildingProductModule
    {
    }
    
    public class KitchenProductModule : BuildingProductModule, IKitchenProductModule
    {
        public KitchenProductModule(
            Transform senderTransform,
            Transform receiverTransform,
            IBuildingStatsModule buildingStatsModule,
            IKitchenInventoryModule kitchenInventoryModule,
            List<Tuple<EMaterialType, EItemType>> inputItemKey,
            List<Tuple<EMaterialType, EItemType>> outputItemKey)
            : base(senderTransform, receiverTransform, buildingStatsModule, kitchenInventoryModule, inputItemKey, outputItemKey)
        {
        }
    }
}