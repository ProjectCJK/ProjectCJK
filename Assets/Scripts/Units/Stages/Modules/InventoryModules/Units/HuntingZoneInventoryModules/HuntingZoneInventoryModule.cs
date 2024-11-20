using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.HuntingZoneInventoryModules
{
    public interface IHuntingZoneInventoryModule : IBuildingInventoryModule
    {
        public event Action<int> OnMoneyReceived;
    }

    public class HuntingZoneInventoryModule : BuildingInventoryModule, IHuntingZoneInventoryModule
    {
        protected override Dictionary<string, int> Inventory
        {
            get
            {
                if (!GameManager.Instance.ES3Saver.BuildingInputItems.ContainsKey(_huntingZoneKey))
                {
                    GameManager.Instance.ES3Saver.BuildingInputItems.TryAdd(_huntingZoneKey, new Dictionary<string, int>());
                }
                
                return GameManager.Instance.ES3Saver.BuildingInputItems[_huntingZoneKey];
            }
            set
            {
                if (!GameManager.Instance.ES3Saver.BuildingInputItems.ContainsKey(_huntingZoneKey))
                {
                    GameManager.Instance.ES3Saver.BuildingInputItems.TryAdd(_huntingZoneKey, new Dictionary<string, int>());
                }

                if (!GameManager.Instance.ES3Saver.BuildingInputItems[_huntingZoneKey].ContainsKey(value.Keys.ToString()))
                {
                    GameManager.Instance.ES3Saver.BuildingInputItems[_huntingZoneKey].TryAdd(_huntingZoneKey, int.Parse(value.Values.ToString()));
                }
            }
        }

        private readonly string _huntingZoneKey;
        
        public HuntingZoneInventoryModule(Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            UpgradableBuildingStatsModule upgradableBuildingStatsModule,
            string inputItemKey,
            string outputItemKey,
            string huntingZoneKey) : base(senderTransform, receiverTransform, itemFactory, upgradableBuildingStatsModule,
            inputItemKey, outputItemKey)
        {
            _huntingZoneKey = huntingZoneKey;
        }

        public event Action<int> OnMoneyReceived;

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            if (Enum.TryParse(inputItemKey, out ECurrencyType currencyType))
                switch (currencyType)
                {
                    case ECurrencyType.Money:
                        OnMoneyReceived?.Invoke(item.Count);
                        break;
                }

            ItemFactory.ReturnItem(item);
        }
    }
}