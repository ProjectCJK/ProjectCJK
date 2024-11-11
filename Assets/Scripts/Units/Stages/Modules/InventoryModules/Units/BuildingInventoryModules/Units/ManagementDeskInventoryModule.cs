using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IManagementDeskInventoryModule : IBuildingInventoryModule
    {
    }

    public class ManagementDeskInventoryModule : BuildingInventoryModule, IManagementDeskInventoryModule
    {
        public ManagementDeskInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            ManagementDeskStatsModule buildingStatsModule,
            IItemFactory itemFactory,
            string inputItemKey, string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, buildingStatsModule, inputItemKey, outputItemKey)
        {
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey, item.Count);
            PushSpawnedItem(ReceiverTransform, item);
        }
    }
}