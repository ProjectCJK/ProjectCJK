using ScriptableObjects.Scripts.Buildings.Units;
using Units.Modules.StatsModules.Units.Buildings.Abstract;

namespace Units.Modules.StatsModules.Units.Buildings.Units
{
    public interface IKitchenStatsModule : IBuildingStatsModule
    {
        public int MaxMaterialInventorySize { get; }
    }
    
    public class KitchenStatsModule : BuildingStatsModule, IKitchenStatsModule
    {
        public int MaxMaterialInventorySize => _kitchenDataSo.BaseMaterialInventorySize;

        private readonly KitchenDataSO _kitchenDataSo;
        
        public KitchenStatsModule(KitchenDataSO kitchenDataSo) : base(kitchenDataSo)
        {
            _kitchenDataSo = kitchenDataSo;
        }
    }
}