using ScriptableObjects.Scripts.Buildings.Abstract;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IDeliveryLodgingStatsModule : IBuildingStatsModule
    {
        public int BaseMaxDeliveryManCount { get; }
    }
    
    public class DeliveryLodgingStatsModule : BuildingStatsModule, IDeliveryLodgingStatsModule
    {
        public int BaseMaxDeliveryManCount => _deliveryLodgingDataSo.BaseMaxDeliveryManCount;

        private readonly DeliveryLodgingDataSO _deliveryLodgingDataSo;
        
        public DeliveryLodgingStatsModule(DeliveryLodgingDataSO buildingDataSo) : base(buildingDataSo)
        {
            _deliveryLodgingDataSo = buildingDataSo;
        }
    }
}