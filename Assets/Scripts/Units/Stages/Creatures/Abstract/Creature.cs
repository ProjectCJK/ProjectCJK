using Interfaces;
using Units.Stages.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Creatures.Abstract
{
    public interface IBaseCreature : IInitializable
    {
        
    }
    
    public abstract class Creature : MonoBehaviour, IBaseCreature
    {
        public abstract ECreatureType CreatureType { get; }

        public abstract void Initialize();
    }
}