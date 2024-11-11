using System.Linq;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IDeliveryLodgingStatsModule : IBuildingStatsModule
    {
    }

    public class DeliveryLodgingStatsModule : UpgradableBuildingStatsModule, IDeliveryLodgingStatsModule
    {
        public sealed override string BuildingKey { get; protected set; }
        
        public override string[,] BuildingData => DataManager.Instance.DeliveryLodgingData.GetData();
        public override string[,] BuildingOption1ValueData => DataManager.Instance.DeliveryLodgingOption1ValueData.GetData();
        public override string[,] BuildingOption2ValueData => DataManager.Instance.DeliveryLodgingOption2ValueData.GetData();
        public override string[,] BuildingOption1CostData => DataManager.Instance.DeliveryLodgingOption1CostData.GetData();
        public override string[,] BuildingOption2CostData => DataManager.Instance.DeliveryLodgingOption2CostData.GetData();

        public DeliveryLodgingStatsModule(DeliveryLodgingDataSO deliveryLodgingDataSo)
            : base(deliveryLodgingDataSo)
        {
            BuildingKey = ParserModule.ParseEnumToString(BuildingType);

            UpdateDefaultValue();
            UpdateBuildingStatsModule();
        }
        
        protected override void UpdateValue() { }
    }
}