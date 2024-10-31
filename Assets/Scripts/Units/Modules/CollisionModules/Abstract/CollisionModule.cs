using Interfaces;
using UnityEngine;

namespace Units.Modules.CollisionModules.Abstract
{
    public interface IInteractionProperty
    {
        public float WaitingTime { get; }
    }
    
    public interface ICollisionModule
    {
    
    }
    
    public enum ECollisionType
    {
        None,
        TradeZone,
        UpgradeZone,
        HuntingZone,
        SpawnZone
    }
    
    public class CollisionModule : ICollisionModule
    {
        private const string tradeZoneLayer = "TradeZone";
        private const string upgradeZoneLayer = "UpgradeZone";
        private const string HuntingZoneLayer = "HuntingZone";
        private const string SpawnZoneLayer = "SpawnZone";

        private readonly int tradeZoneLayerMask = LayerMask.NameToLayer(tradeZoneLayer);
        private readonly int upgradeZoneLayerMask = LayerMask.NameToLayer(upgradeZoneLayer);
        private readonly int huntingZoneLayerMask = LayerMask.NameToLayer(HuntingZoneLayer);
        private readonly int spawnZoneLayerMask = LayerMask.NameToLayer(SpawnZoneLayer);
        
        protected ECollisionType CheckLayer(int layer)
        {
            return layer switch
            {
                _ when layer == tradeZoneLayerMask => ECollisionType.TradeZone,
                _ when layer == upgradeZoneLayerMask => ECollisionType.UpgradeZone,
                _ when layer == huntingZoneLayerMask => ECollisionType.HuntingZone,
                _ when layer == spawnZoneLayerMask => ECollisionType.SpawnZone,
                _ => ECollisionType.None
            };
        }
    }
}