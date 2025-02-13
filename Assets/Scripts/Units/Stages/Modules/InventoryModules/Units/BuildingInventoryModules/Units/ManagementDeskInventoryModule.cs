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
    public interface IManagementDeskInventoryModule : IBuildingInventoryModule
    {
    }

    public class ManagementDeskInventoryModule : BuildingInventoryModule, IManagementDeskInventoryModule
    {
        public override Dictionary<string, int> Inventory
        {
            get
            {
                if (!GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey(_managementDeskStatsModule.BuildingKey))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems.TryAdd(_managementDeskStatsModule.BuildingKey, new Dictionary<string, int>());
                }
                
                return GameManager.Instance.ES3Saver.BuildingOutputItems[_managementDeskStatsModule.BuildingKey];
            }
            set
            {
                if (!GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey(_managementDeskStatsModule.BuildingKey))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems.TryAdd(_managementDeskStatsModule.BuildingKey, new Dictionary<string, int>());
                }

                if (!GameManager.Instance.ES3Saver.BuildingOutputItems[_managementDeskStatsModule.BuildingKey].ContainsKey(value.Keys.ToString()))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems[_managementDeskStatsModule.BuildingKey].TryAdd(_managementDeskStatsModule.BuildingKey, int.Parse(value.Values.ToString()));
                }
            }
        }
        
        private readonly ManagementDeskStatsModule _managementDeskStatsModule;
        
        public ManagementDeskInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            ManagementDeskStatsModule managementDeskStatsModule,
            IItemFactory itemFactory,
            string inputItemKey, string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, managementDeskStatsModule, inputItemKey, outputItemKey)
        {
            _managementDeskStatsModule = managementDeskStatsModule;
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            TempMoney -= item.Count;
            AddItem(inputItemKey, item.Count);
            ItemFactory.ReturnItem(item);
            OnUpdateStackedItem?.Invoke(Inventory[inputItemKey]);
        }
    }
}