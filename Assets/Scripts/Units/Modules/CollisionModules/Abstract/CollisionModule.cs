using Interfaces;
using UnityEngine;

namespace Units.Modules.CollisionModules.Abstract
{
    public interface ICollisionModule : IInitializable
    {
    
    }
    
    public class CollisionModule
    {
        private const string InteractionTradeLayer = "InteractionTrade";
        private const string InteractionUpgradeLayer = "InteractionUpgrade";
        
        protected int interactionTradeLayerMask;
        protected int interactionUpgradeLayerMask;
        
        public virtual void Initialize()
        {
            interactionTradeLayerMask = LayerMask.NameToLayer(InteractionTradeLayer);
            interactionUpgradeLayerMask = LayerMask.NameToLayer(InteractionUpgradeLayer);
        }
    }
}