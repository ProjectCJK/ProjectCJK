using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IStandStatsModule : IBuildingStatsModule
    {
    }

    public class StandStatsModule : BuildingStatsModule, IStandStatsModule
    {
        public sealed override string BuildingKey { get; protected set; }
        
        public StandStatsModule(StandDataSO standDataSo) : base(standDataSo)
        {
            BuildingKey = ParserModule.ParseEnumToString(BuildingType);
        }
    }
}