using Interfaces;
using UnityEngine;

namespace Units.Modules.MovementModules.Abstract
{
    public interface IMovementModule
    {
        
    }
    
    public interface IMovementProperty
    {
        public float MovementSpeed { get; }
    }
    
    public abstract class MovementModule : IMovementModule
    {
        private const string collisionLayer = "Collision";
        protected readonly int collisionLayerMask = LayerMask.GetMask(collisionLayer);
    }
}