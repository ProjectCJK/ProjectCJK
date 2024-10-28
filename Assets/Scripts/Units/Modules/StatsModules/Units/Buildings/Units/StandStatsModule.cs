using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Modules.StatsModules.Abstract;

namespace Units.Modules.StatsModules.Units.Buildings
{
    public interface IStandStatsModule : IBuildingStatsModule
    {
    
    }
    
    public class StandStatsModule : BuildingStatsModule, IStandStatsModule
    {
        public StandStatsModule(StandDataSO standDataSo) : base(standDataSo)
        {
            
        }
    }
}