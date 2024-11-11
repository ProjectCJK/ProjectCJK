using System.Linq;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IWareHouseStatsModule : IBuildingStatsModule
    {
    }

    public class WareHouseStatsModule : UpgradableBuildingStatsModule, IWareHouseStatsModule
    {
        public sealed override string BuildingKey { get; protected set; }
        
        public override string[,] BuildingData => DataManager.Instance.WareHouseData.GetData();
        public override string[,] BuildingOption1ValueData => DataManager.Instance.WareHouseOption1ValueData.GetData();
        public override string[,] BuildingOption2ValueData => DataManager.Instance.WareHouseOption2ValueData.GetData();
        public override string[,] BuildingOption1CostData => DataManager.Instance.WareHouseOption1CostData.GetData();
        public override string[,] BuildingOption2CostData => DataManager.Instance.WareHouseOption2CostData.GetData();

        public WareHouseStatsModule(
            WareHouseDataSO wareHouseDataSo,
            WareHouseCustomSetting wareHouseCustomSetting) : base(wareHouseDataSo)
        {
            BuildingKey = ParserModule.ParseEnumToString(BuildingType);

            UpdateDefaultValue();
            UpdateBuildingStatsModule();
        }

        protected override void UpdateValue() { }
    }
}