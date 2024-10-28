using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Modules.StatsModules.Abstract;

namespace Units.Modules.StatsModules.Units.Buildings
{
    public interface IKitchenStatsModule : IBuildingStatsModule
    {
        
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