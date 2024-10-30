using ScriptableObjects.Scripts.Buildings.Abstract;
using Units.Modules.StatsModules.Units.Buildings.Abstract;

namespace Units.Modules.StatsModules.Units.Buildings.Units
{
    public interface IManagementDeskStatsModule : IBuildingStatsModule
    {
        
    }
    
    public class ManagementDeskStatsModule : BuildingStatsModule, IManagementDeskStatsModule
    {
        public ManagementDeskStatsModule(BuildingDataSO buildingDataSo) : base(buildingDataSo)
        {
        }
    }
}