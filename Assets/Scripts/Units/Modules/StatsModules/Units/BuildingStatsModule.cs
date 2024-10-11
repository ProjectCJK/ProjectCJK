using ScriptableObjects.Scripts;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.StatsModules.Abstract;

namespace Units.Modules.StatsModules.Units
{
    public interface IBuildingStatsModule : IInventoryProperty, IProductProperty
    {
        
    }

    public class BuildingStatsModule : StatsModule, IBuildingStatsModule
    {
        public float ProductLeadTime => _buildingStatSo.productLeadTime;
        public int MaxInventorySize => _buildingStatSo.BaseInventorySize;
        
        private readonly BuildingStatSO _buildingStatSo;

        public BuildingStatsModule(BuildingStatSO buildingStatSo)
        {
            _buildingStatSo = buildingStatSo;
        }
    }
}