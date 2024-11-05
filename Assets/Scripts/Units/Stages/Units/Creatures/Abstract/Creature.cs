using Interfaces;
using Units.Stages.Modules.FSMModules.Units;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Abstract
{
    public interface ICreature
    {
        public ECreatureType CreatureType { get; }
        public Transform Transform { get; }
    }
    
    public interface ICreatureTypeProperty
    {
        public ECreatureType CreatureType { get; }
    }
    
    public abstract class Creature : MonoBehaviour, ICreature
    {
        [SerializeField] protected Transform spriteTransform;
        private Transform _transform;

        public abstract ECreatureType CreatureType { get; }
        public abstract Animator Animator { get; protected set; }
        public abstract Transform Transform { get; }
    }
}