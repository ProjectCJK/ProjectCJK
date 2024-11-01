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
        protected readonly int collisionLayerMask = LayerMaskParserModule.CollisionLayerMask;
    }
}