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
            WareHouseDataSO wareHouseDataSo) : base(wareHouseDataSo)
        {
            BuildingKey = ParserModule.ParseEnumToString(BuildingType);
            
            GameManager.Instance.ES3Saver.CurrentBuildingLevel.TryAdd(BuildingKey, 1);
            GameManager.Instance.ES3Saver.CurrentBuildingOption1Level.TryAdd(BuildingKey, 1);
            GameManager.Instance.ES3Saver.CurrentBuildingOption2Level.TryAdd(BuildingKey, 1);

            UpdateDefaultValue();
            UpdateBuildingStatsModule();
        }

        protected override void UpdateValue() { }
    }
}