using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.ProductModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using UnityEngine;

namespace Units.Stages.Modules.ProductModules.Units
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