using System;
using System.Collections.Generic;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.CollisionModules.Units;
using Units.Modules.FactoryModules.Units;
using Units.Modules.FSMModules.Units;
using Units.Modules.InventoryModules.Units.CreatureInventoryModules.Units;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IGuest : IBaseCreature, IPoolable, IRegisterReference<GuestDataSO, IItemFactory>, IInitializable<Vector3, Action>
    {
        public void SetDestinations(List<Tuple<string, Transform>> destinations);
        public void SetTargetPurchaseQuantity(int targetPurchaseQuantity);
    }
    
    public class Guest : Creature, IGuest
    {
        private event Action OnReturnGuest;
        
        public override ECreatureType CreatureType => _guestStatModule.Type;
        public override Animator Animator => _animator;
        public override Transform Transform => transform;
        
        protected override CreatureStateMachine creatureStateMachine { get; set; }

        private IGuestInventoryModule _guestInventoryModule;
        private IGuestStatModule _guestStatModule;
        private IGuestMovementModule _guestMovementModule;
        private IGuestCollisionModule _guestCollisionModule;

        private List<Tuple<string, Transform>> _destinations;
        private int _destinationIndex;
        
        private Animator _animator;

        public void RegisterReference(GuestDataSO guestDataSo, IItemFactory itemFactory)
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            
            _guestStatModule = new GuestStatModule(guestDataSo);
            _guestMovementModule = new GuestMovementModule(this, _guestStatModule);
            _guestCollisionModule = new GuestCollisionModule(_guestStatModule);
            _guestInventoryModule = new GuestInventoryModule(transform, transform, _guestStatModule, itemFactory, CreatureType);

            _guestCollisionModule.OnCompareWithTarget += HandleOnCompareWithTarget;
            _guestCollisionModule.OnTriggerTradeZone += HandleOnTriggerTradeZone;

            _guestInventoryModule.OnTargetQuantityReceived += HandleOnTargetQuantityReceived;
        }
        
        public void Initialize(Vector3 startPoint, Action action)
        {
            _destinationIndex = 0;
            OnReturnGuest = action;
            
            _guestMovementModule.Initialize(startPoint);
        }

        private void Update()
        {
            _guestInventoryModule.Update();
        }

        private void FixedUpdate()
        {
            _guestMovementModule.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _guestCollisionModule.OnTriggerEnter2D(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            _guestCollisionModule.OnTriggerStay2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _guestCollisionModule.OnTriggerExit2D(other);
        }
        
        public void SetTargetPurchaseQuantity(int targetPurchaseQuantity)
        {
            _guestStatModule.SetMaxInventorySize(targetPurchaseQuantity);
        }
        
        public void SetDestinations(List<Tuple<string, Transform>> destinations)
        {
            _destinations = destinations;
            _guestMovementModule.SetDestination(_destinations[_destinationIndex].Item2.position);
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

        private bool HandleOnCompareWithTarget(string buildingKey)
        {
            if (string.Equals(buildingKey, _destinations[_destinationIndex].Item1))
            {
                _guestMovementModule.ActivateNavMeshAgent(false);
                return true;
            }

            return false;
        }
        
        private void HandleOnTriggerTradeZone(IInteractionTrade interactionZone, bool isConnected)
        {
            _guestInventoryModule.RegisterItemReceiver(interactionZone, isConnected); 
        }

        private void HandleOnTargetQuantityReceived()
        {
            _guestMovementModule.SetDestination(_destinations[++_destinationIndex].Item2.position);
        }
    }
}