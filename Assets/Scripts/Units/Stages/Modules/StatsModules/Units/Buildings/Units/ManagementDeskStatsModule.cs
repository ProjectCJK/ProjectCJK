using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.ProductModules.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IManagementDeskStatsModule : IBuildingStatsModule, IProductProperty
    {
        
    }
    
    public class ManagementDeskStatsModule : BuildingStatsModule, IManagementDeskStatsModule
    {
        public EBuildingType BuildingType => _managementDeskDataSo.BuildingType;
        public float BaseProductLeadTime { get; }
        private readonly ManagementDeskDataSO _managementDeskDataSo;
        
        public ManagementDeskStatsModule(ManagementDeskDataSO managementDeskDataSo) : base(managementDeskDataSo)
        {
            _managementDeskDataSo = managementDeskDataSo;
        }
    }
}