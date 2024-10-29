using System;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.FSMModules.Units;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IGuest : IPoolable, IRegisterReference<GuestDataSO>, IInitializable<Vector3>
    {
    }
    
    public class Guest : Creature, IGuest
    {
        public override ECreatureType CreatureType => _guestStatModule.Type;
        public override Animator Animator { get; protected set; }
        protected override CreatureStateMachine creatureStateMachine { get; set; }
        
        private IGuestStatModule _guestStatModule;
        private IGuestMovementModule _guestMovementModule;
        
        public void RegisterReference(GuestDataSO guestDataSo)
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            
            _guestStatModule = new GuestStatModule(guestDataSo);
            _guestMovementModule = new GuestMovementModule(this, _guestStatModule);
        }
        
        public void Initialize(Vector3 targetPosition)
        {
            _guestMovementModule.Initialize(targetPosition);
        }

        private void Update()
        {
            _guestMovementModule.Update();
        }

        public void Create()
        {
            SetActive(false);
        }

        public void GetFromPool()
        {
            SetActive(true);
        }

        public void ReturnToPool()
        {
            SetActive(false);
        }
        
        private void SetActive(bool value)
        {
            if (gameObject.activeInHierarchy != value) gameObject.SetActive(value);
        }
    }
}