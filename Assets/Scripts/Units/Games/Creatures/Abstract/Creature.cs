using Interfaces;
using Units.Games.Creatures.Enums;
using UnityEngine;

namespace Units.Games.Creatures.Abstract
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