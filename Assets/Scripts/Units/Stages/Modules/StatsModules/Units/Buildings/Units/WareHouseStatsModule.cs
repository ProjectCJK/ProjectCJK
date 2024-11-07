using ScriptableObjects.Scripts.Buildings.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IWareHouseStatsModule : IBuildingStatsModule
    {
    }
    
    public class WareHouseStatsModule : BuildingStatsModule, IWareHouseStatsModule
    {
        public WareHouseStatsModule(BuildingDataSO buildingDataSo) : base(buildingDataSo)
        {
        }
    }
}