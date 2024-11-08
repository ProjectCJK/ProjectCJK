using Interfaces;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IHunter : ICreature, IRegisterReference<HunterDataSO, IItemFactory>, IInitializable<Vector3>
    {
        
    }
    
    public class Hunter : Creature, IHunter
    {
        public override ECreatureType CreatureType { get; }
        public override Transform Transform { get; }
        public override Animator Animator { get; protected set; }
        
        public void RegisterReference(HunterDataSO hunterDataSo, IItemFactory itemFactory)
        {
            var navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
        }

        public void Initialize(Vector3 instance1)
        {
            
        }
    }
}