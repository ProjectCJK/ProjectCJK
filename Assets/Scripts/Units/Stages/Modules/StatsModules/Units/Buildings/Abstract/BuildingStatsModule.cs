using ScriptableObjects.Scripts.Buildings.Abstract;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.ProductModules.Abstract;
using Units.Stages.Modules.StatsModules.Abstract;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Abstract
{
    public interface IBuildingStatsModule : IInventoryProperty, IProductProperty
    {
        
    }

    public abstract class BuildingStatsModule : StatsModule, IBuildingStatsModule
    {
        public float BaseProductLeadTime => _buildingDataSo.BaseProductLeadTime;
        public int MaxProductInventorySize => _buildingDataSo.BaseProductInventorySize;
        
        private readonly BuildingDataSO _buildingDataSo;

        protected BuildingStatsModule(BuildingDataSO buildingDataSo)
        {
            _buildingDataSo = buildingDataSo;
        }
    }
}