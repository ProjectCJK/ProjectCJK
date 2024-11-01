using Interfaces;
using UnityEngine;

namespace Units.Modules.MovementModules.Abstract
{
    public interface IMovementModule { }

    public interface IMovementProperty
    {
        float MovementSpeed { get; }
    }

    public abstract class MovementModule : IMovementModule
    {

    }
}