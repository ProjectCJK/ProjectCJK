using ScriptableObjects.Scripts.Buildings.Abstract;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;

namespace Units.Modules.StatsModules.Units.Buildings.Units
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