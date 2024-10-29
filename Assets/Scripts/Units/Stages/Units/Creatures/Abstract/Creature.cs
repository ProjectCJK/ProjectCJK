using Interfaces;
using Units.Modules.FSMModules.Units;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Abstract
{
    public interface IBaseCreature
    {
        public Transform Transform { get; }
    }
    
    public interface ICreatureTypeProperty
    {
        public ECreatureType Type { get; }
    }
    
    public abstract class Creature : MonoBehaviour, IBaseCreature
    {
        [SerializeField] protected Transform spriteTransform;
        private Transform _transform;

        public abstract ECreatureType CreatureType { get; }
        public abstract Animator Animator { get; }
        public abstract Transform Transform { get; }
        
        protected abstract CreatureStateMachine creatureStateMachine { get; set; }
    }
}