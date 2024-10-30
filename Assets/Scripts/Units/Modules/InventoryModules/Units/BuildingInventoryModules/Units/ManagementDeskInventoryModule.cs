using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IManagementDeskInventoryModule : IBuildingInventoryModule
    {
        
    }
    public class ManagementDeskInventoryModule : BuildingInventoryModule, IManagementDeskInventoryModule
    {
        public ManagementDeskInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            IBuildingStatsModule buildingStatsModule,
            string inputItemKey, string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, buildingStatsModule, inputItemKey, outputItemKey)
        {
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey);
            PushSpawnedItem(ReceiverTransform, item);
        }
    }
}