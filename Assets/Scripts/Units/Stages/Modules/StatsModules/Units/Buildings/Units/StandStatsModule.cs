using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
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