using System;
using System.Linq;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using TMPro;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IKitchenStatsModule : IBuildingStatsModule
    {
    }

    public class KitchenStatsModule : UpgradableBuildingStatsModule, IKitchenStatsModule
    {
        public sealed override string BuildingKey { get; protected set; }
        
        public readonly string InputItemKey;
        public readonly EItemType InputItemType;

        public readonly EItemType ItemType;
        public readonly EMaterialType MaterialType;
        public readonly string OutputItemKey;
        public readonly EItemType OutputItemType;
        public readonly EStageMaterialType StageMaterialType;
        
        public override string[,] BuildingData => DataManager.Instance.KitchenData.GetData();
        public override string[,] BuildingOption1ValueData => DataManager.Instance.KitchenOption1ValueData.GetData();
        public override string[,] BuildingOption2ValueData => DataManager.Instance.KitchenOption2ValueData.GetData();
        public override string[,] BuildingOption1CostData => DataManager.Instance.KitchenOption1CostData.GetData();
        public override string[,] BuildingOption2CostData => DataManager.Instance.KitchenOption2CostData.GetData();


        public KitchenStatsModule(KitchenDataSO kitchenDataSo, KitchenCustomSetting kitchenCustomSetting) : base(kitchenDataSo)
        {
            MaterialType = kitchenCustomSetting.MaterialType;
            InputItemType = kitchenCustomSetting.InputItemType;
            OutputItemType = kitchenCustomSetting.OutputItemType;

            BuildingKey = ParserModule.ParseEnumToString(BuildingType, MaterialType);
            InputItemKey = ParserModule.ParseEnumToString(InputItemType, MaterialType);
            OutputItemKey = ParserModule.ParseEnumToString(OutputItemType, MaterialType);

            ItemType = OutputItemType;
            StageMaterialType = VolatileDataManager.Instance.MaterialMappings[MaterialType];
            
            UpdateDefaultValue();
            UpdateBuildingStatsModule();
        }
        
        protected override void UpdateValue()
        {
            if (VolatileDataManager.Instance.ItemPrices.ContainsKey(OutputItemKey))
            {
                VolatileDataManager.Instance.ItemPrices[OutputItemKey] = (int)CurrentBuildingOption1Value;    
            }
            else
            {
                VolatileDataManager.Instance.ItemPrices.TryAdd(OutputItemKey, (int)CurrentBuildingOption1Value);   
            }
        }
    }
}