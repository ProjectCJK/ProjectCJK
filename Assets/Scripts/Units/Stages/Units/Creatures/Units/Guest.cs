using System;
using System.Collections.Generic;
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
    public interface IGuest : IBaseCreature, IPoolable, IRegisterReference<GuestDataSO>, IInitializable<Action>
    {
        public void SetDestinations(Vector3 startPosition, List<Transform> destinations);
    }
    
    public class Guest : Creature, IGuest
    {
        private event Action OnReturnGuest;
        
        public override ECreatureType CreatureType => _guestStatModule.Type;
        public override Animator Animator => _animator;
        public override Transform Transform => transform;
        
        protected override CreatureStateMachine creatureStateMachine { get; set; }
        
        private IGuestStatModule _guestStatModule;
        private IGuestMovementModule _guestMovementModule;
        
        private Animator _animator;

        public void RegisterReference(GuestDataSO guestDataSo)
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            
            _guestStatModule = new GuestStatModule(guestDataSo);
            _guestMovementModule = new GuestMovementModule(this, _guestStatModule);
        }
        
        public void Initialize(Action action)
        {
            OnReturnGuest = action;
        }

        public void SetDestinations(Vector3 startPosition, List<Transform> destinations)
        {
            _guestMovementModule.Initialize(startPosition, destinations);
        }

        private void FixedUpdate()
        {
            _guestMovementModule.FixedUpdate();
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