using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IStandStatsModule : IBuildingStatsModule
    {
    }

    public class StandStatsModule : BuildingStatsModule, IStandStatsModule
    {
        public readonly EMaterialType MaterialType;
        public sealed override string BuildingKey { get; protected set; }
        
        public StandStatsModule(StandDataSO standDataSo, StandCustomSetting standCustomSetting) : base(standDataSo)
        {
            MaterialType = standCustomSetting.MaterialType;
            BuildingKey = ParserModule.ParseEnumToString(BuildingType, MaterialType);
        }
    }
}