using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IStandInventoryModule : IBuildingInventoryModule
    {
        public event Action<int> OnMoneyReceived;
    }

    public class StandInventoryModule : BuildingInventoryModule, IStandInventoryModule
    {
        public override Dictionary<string, int> Inventory
        {
            get
            {
                if (!GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey(_standStatsModule.BuildingKey))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems.TryAdd(_standStatsModule.BuildingKey, new Dictionary<string, int>());
                }
                
                return GameManager.Instance.ES3Saver.BuildingOutputItems[_standStatsModule.BuildingKey];
            }
            set
            {
                if (!GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey(_standStatsModule.BuildingKey))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems.TryAdd(_standStatsModule.BuildingKey, new Dictionary<string, int>());
                }

                if (!GameManager.Instance.ES3Saver.BuildingOutputItems[_standStatsModule.BuildingKey].ContainsKey(value.Keys.ToString()))
                {
                    GameManager.Instance.ES3Saver.BuildingOutputItems[_standStatsModule.BuildingKey].TryAdd(_standStatsModule.BuildingKey, int.Parse(value.Values.ToString()));
                }
            }
        }
        
        private readonly StandStatsModule _standStatsModule;
        
        public StandInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            StandStatsModule standStatsModule,
            IItemFactory itemFactory,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, standStatsModule, inputItemKey, outputItemKey)
        {
            _standStatsModule = standStatsModule;
        }

        public event Action<int> OnMoneyReceived;

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            if (Enum.TryParse(inputItemKey, out ECurrencyType currencyType))
            {
                switch (currencyType)
                {
                    case ECurrencyType.Money:
                        TempMoney -= item.Count;
                        OnMoneyReceived?.Invoke(item.Count);
                        break;
                }
                
                ItemFactory.ReturnItem(item);
            }
            else
            {
                AddItem(inputItemKey, item.Count);
                ItemFactory.ReturnItem(item);
                OnUpdateStackedItem?.Invoke(Inventory[inputItemKey]);
            }
        }
    }
}