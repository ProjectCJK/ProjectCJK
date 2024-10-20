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
    
    public abstract class MovementModule : IMovementModule
    {
        public abstract void Initialize();
    }
}