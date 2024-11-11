using ScriptableObjects.Scripts.Buildings.Abstract;
using Units.Stages.Modules.StatsModules.Abstract;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Abstract
{
    public interface IBuildingStatsModule
    {
    }

    public abstract class BuildingStatsModule : StatsModule, IBuildingStatsModule
    {
        private readonly BuildingDataSO _buildingDataSo;

        protected BuildingStatsModule(BuildingDataSO buildingDataSo)
        {
            _buildingDataSo = buildingDataSo;
        }

        public int MaxProductInventorySize => _buildingDataSo.BaseProductInventorySize;
    }
}