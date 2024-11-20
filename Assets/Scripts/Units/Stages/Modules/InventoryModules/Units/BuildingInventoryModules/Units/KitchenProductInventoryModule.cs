using System;
using System.Collections.Generic;
using Managers;
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
        public override Dictionary<string, int> Inventory
        {
                get
            {
                if (!GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey(_kitchenStatsModule.BuildingKey))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems.TryAdd(_kitchenStatsModule.BuildingKey, new Dictionary<string, int>());
                }
                
                return GameManager.Instance.ES3Saver.BuildingOutputItems[_kitchenStatsModule.BuildingKey];
            }
            set
            {
                if (!GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey(_kitchenStatsModule.BuildingKey))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems.TryAdd(_kitchenStatsModule.BuildingKey, new Dictionary<string, int>());
                }

                if (!GameManager.Instance.ES3Saver.BuildingOutputItems[_kitchenStatsModule.BuildingKey].ContainsKey(value.Keys.ToString()))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems[_kitchenStatsModule.BuildingKey].TryAdd(_kitchenStatsModule.BuildingKey, int.Parse(value.Values.ToString()));
                }
            }
        }
        
        private readonly KitchenStatsModule _kitchenStatsModule;
        
        public KitchenProductInventoryModule(Transform senderTransform,
            Transform receiverTransform,
            KitchenStatsModule kitchenStatsModule,
            IItemFactory itemFactory,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, kitchenStatsModule, inputItemKey, outputItemKey)
        {
            _kitchenStatsModule = kitchenStatsModule;
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey, item.Count);
            ItemFactory.ReturnItem(item);
            OnUpdateStackedItem?.Invoke(Inventory[inputItemKey]);
        }
    }
}