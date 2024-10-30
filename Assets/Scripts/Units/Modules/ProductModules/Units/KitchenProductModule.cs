using System;
using System.Collections.Generic;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.ProductModules.Abstract;
using Units.Modules.StatsModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Buildings.Abstract;
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
        public KitchenProductModule(Transform senderTransform,
            Transform receiverTransform,
            IBuildingStatsModule buildingStatsModule,
            IKitchenMaterialInventoryModule kitchenMaterialInventoryModule,
            IKitchenProductInventoryModule kitchenProductInventoryModule,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, buildingStatsModule, kitchenMaterialInventoryModule, kitchenProductInventoryModule, inputItemKey, outputItemKey)
        {
        }
    }
}