using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IKitchenProductInventoryModule : IBuildingInventoryModule
    {
    }

    public class KitchenProductInventoryModule : BuildingInventoryModule, IKitchenProductInventoryModule
    {
        public KitchenProductInventoryModule(Transform senderTransform,
            Transform receiverTransform,
            KitchenStatsModule kitchenStatsModule,
            IItemFactory itemFactory,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, kitchenStatsModule, inputItemKey, outputItemKey)
        {
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey, item.Count);
            PushSpawnedItem(ReceiverTransform, item);
        }
    }
}