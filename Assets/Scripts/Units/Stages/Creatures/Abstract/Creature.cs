using Interfaces;
using Modules.DesignPatterns.FSMs.Modules;
using Units.Modules.FSMModules.Units;
using Units.Stages.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Creatures.Abstract
{
    public interface IBaseCreature : IInitializable
    {
        
    }
    
    public abstract class Creature : MonoBehaviour, IBaseCreature
    {
        [SerializeField] protected Transform spriteTransform;

        public abstract ECreatureType CreatureType { get; }
        public abstract Animator Animator { get; protected set; }

        protected abstract CreatureStateMachine creatureStateMachine { get; set; }

        public abstract void Initialize();
    }
}