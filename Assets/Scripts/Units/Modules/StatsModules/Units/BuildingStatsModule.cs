using ScriptableObjects.Scripts;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.StatsModules.Abstract;

namespace Units.Modules.StatsModules.Units
{
    public interface IBuildingStatsModule : IInventoryProperty
    {
        
    }
    
    public class BuildingStatsModule : StatsModule, IBuildingStatsModule
    {
        public int MaxInventorySize => _buildingStatSo.BaseInventorySize;
        
        private BuildingStatSO _buildingStatSo;

        public BuildingStatsModule(BuildingStatSO buildingStatSo)
        {
            _buildingStatSo = buildingStatSo;
        }
    }
}