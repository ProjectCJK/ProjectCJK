using ScriptableObjects.Scripts.Buildings;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.ProductModules;
using Units.Modules.StatsModules.Abstract;

namespace Units.Modules.StatsModules.Units
{
    public interface IBuildingStatsModule : IInventoryProperty, IProductProperty
    {
        
    }

    public class BuildingStatsModule : StatsModule, IBuildingStatsModule
    {
        public float ProductLeadTime => _buildingDataSo.productLeadTime;
        public int MaxInventorySize => _buildingDataSo.BaseInventorySize;
        
        private readonly BuildingDataSO _buildingDataSo;

        public BuildingStatsModule(BuildingDataSO buildingDataSo)
        {
            _buildingDataSo = buildingDataSo;
        }
    }
}