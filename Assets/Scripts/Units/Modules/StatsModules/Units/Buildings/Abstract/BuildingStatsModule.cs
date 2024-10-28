using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Buildings.Abstract;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.ProductModules;
using Units.Modules.ProductModules.Abstract;

namespace Units.Modules.StatsModules.Abstract
{
    public interface IBuildingStatsModule : IInventoryProperty, IProductProperty
    {
        
    }

    public abstract class BuildingStatsModule : StatsModule, IBuildingStatsModule
    {
        public float ProductLeadTime => _buildingDataSo.productLeadTime;
        public int MaxProductInventorySize => _buildingDataSo.BaseProductInventorySize;
        
        private readonly BuildingDataSO _buildingDataSo;

        protected BuildingStatsModule(BuildingDataSO buildingDataSo)
        {
            _buildingDataSo = buildingDataSo;
        }
    }
}