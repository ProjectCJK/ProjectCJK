using System;
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
        private event Action<bool> OnKitchenProductExisted;
        
        public KitchenProductInventoryModule(Transform senderTransform,
            Transform receiverTransform,
            IKitchenStatsModule kitchenStatsModule,
            IItemFactory itemFactory,
            string inputItemKey,
            string outputItemKey, Action<bool> onKitchenProductExisted)
            : base(senderTransform, receiverTransform, itemFactory, kitchenStatsModule, inputItemKey, outputItemKey)
        {
            OnKitchenProductExisted = onKitchenProductExisted;
        }

        protected override void SendItem()
        {
            base.SendItem();
            
            if (CurrentInventorySize <= 0) OnKitchenProductExisted?.Invoke(false);
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey, item.Count);
            PushSpawnedItem(ReceiverTransform, item);
            
            if (CurrentInventorySize > 0) OnKitchenProductExisted?.Invoke(true);
        }
    }
}