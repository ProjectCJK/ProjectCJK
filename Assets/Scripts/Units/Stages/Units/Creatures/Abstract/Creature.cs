using Interfaces;
using Units.Modules.FSMModules.Units;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Abstract
{
    public interface IBaseCreature
    {
        
    }
    
    public abstract class Creature : MonoBehaviour, IBaseCreature
    {
        [SerializeField] protected Transform spriteTransform;

        public abstract ECreatureType CreatureType { get; }
        public abstract Animator Animator { get; protected set; }

        protected abstract CreatureStateMachine creatureStateMachine { get; set; }
    }
}