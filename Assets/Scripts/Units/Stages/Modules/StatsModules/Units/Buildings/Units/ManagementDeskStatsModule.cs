using System.Linq;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Units;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IManagementDeskStatsModule : IBuildingStatsModule
    {
    }

    public class ManagementDeskStatsModule : UpgradableBuildingStatsModule, IManagementDeskStatsModule
    {
        public sealed override string BuildingKey { get; protected set; }
        
        public readonly string InputItemKey;
        public readonly string OutputItemKey;
        
        public override string[,] BuildingData => DataManager.Instance.ManagementDeskData.GetData();
        public override string[,] BuildingOption1ValueData => DataManager.Instance.ManagementDeskOption1ValueData.GetData();
        public override string[,] BuildingOption2ValueData => DataManager.Instance.ManagementDeskOption2ValueData.GetData();
        public override string[,] BuildingOption1CostData => DataManager.Instance.ManagementDeskOption1CostData.GetData();
        public override string[,] BuildingOption2CostData => DataManager.Instance.ManagementDeskOption2CostData.GetData();

        public ManagementDeskStatsModule(
            ManagementDeskDataSO managementDeskDataSo,
            ManagementDeskCustomSetting managementDeskCustomSetting) : base(managementDeskDataSo)
        {
            BuildingKey = ParserModule.ParseEnumToString(managementDeskDataSo.BuildingType);
            InputItemKey = ParserModule.ParseEnumToString(managementDeskCustomSetting.CurrencyType);
            OutputItemKey = ParserModule.ParseEnumToString(managementDeskCustomSetting.CurrencyType);
            
            UpdateDefaultValue();
            UpdateBuildingStatsModule();
        }
        
        protected override void UpdateValue() { }
    }
}