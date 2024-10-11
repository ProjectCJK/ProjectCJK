using Interfaces;
using UnityEngine;

namespace Units.Modules.MovementModules.Abstract
{
    public interface IMovementModule : IInitializable
    {
        
    }
    
    public interface IMovementProperty
    {
        public float MovementSpeed { get; }
        public float WaitingTime { get; }
    }
    
    public abstract class MovementModule : MonoBehaviour, IMovementModule
    {
        private const string InteractionTradeLayer = "InteractionTrade";
        private const string InteractionUpgradeLayer = "InteractionUpgrade";

        protected int InteractionTradeLayerMask;
        protected int InteractionUpgradeLayerMask;
        
        public virtual void Initialize()
        {
            InteractionTradeLayerMask = LayerMask.NameToLayer(InteractionTradeLayer);
            InteractionUpgradeLayerMask = LayerMask.NameToLayer(InteractionUpgradeLayer);
        }
    }
}