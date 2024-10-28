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
        HuntingZone
    }
    
    public class CollisionModule : ICollisionModule
    {
        private const string tradeZoneLayer = "TradeZone";
        private const string upgradeZoneLayer = "UpgradeZone";
        private const string HuntingZoneLayer = "HuntingZone";
        
        protected readonly int tradeZoneLayerMask = LayerMask.NameToLayer(tradeZoneLayer);
        protected readonly int upgradeZoneLayerMask = LayerMask.NameToLayer(upgradeZoneLayer);
        protected readonly int huntingZoneLayerMask = LayerMask.NameToLayer(HuntingZoneLayer);
    }
}