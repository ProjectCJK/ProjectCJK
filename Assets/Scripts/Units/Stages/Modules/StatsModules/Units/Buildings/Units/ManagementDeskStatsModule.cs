using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IManagementDeskStatsModule : IBuildingStatsModule
    {
        
    }
    
    public class ManagementDeskStatsModule : BuildingStatsModule, IManagementDeskStatsModule
    {
        public EBuildingType BuildingType => _managementDeskDataSo.BuildingType;

        private readonly ManagementDeskDataSO _managementDeskDataSo;
        
        public ManagementDeskStatsModule(ManagementDeskDataSO managementDeskDataSo) : base(managementDeskDataSo)
        {
            _managementDeskDataSo = managementDeskDataSo;
        }
    }
}