using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.CollisionModules.Units;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.FSMModules.Units;
using Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Units;
using Units.Stages.Modules.MovementModules.Units;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IGuest : INPC, IPoolable, IRegisterReference<GuestDataSO, IItemFactory>, IInitializable<Vector3, Action>
    {
        public void SetDestinations(List<Tuple<string, Transform>> destinations);
        public void SetTargetPurchaseQuantity(int targetPurchaseQuantity);
        public void CheckNextDestination();
    }
    
    public class Guest : NPC, IGuest
    {
        private event Action OnReturnGuest;
        
        public override ECreatureType CreatureType => _guestStatModule.CreatureType;
        public override Animator Animator => _animator;
        public override Transform Transform => transform;
        
        protected override CreatureStateMachine creatureStateMachine { get; set; }

        private IGuestInventoryModule _guestInventoryModule;
        private IGuestStatModule _guestStatModule;
        private IGuestMovementModule _guestMovementModule;
        private IGuestCollisionModule _guestCollisionModule;

        private List<Tuple<string, Transform>> _destinations;
        private int _destinationIndex;
        
        private ENPCType NPCType => _guestStatModule.NPCType;
        private Animator _animator;

        private float _elapsedTime;  // 시간 측정 변수
        private bool _waitingTrigger; // 대기 상태 여부
        private bool _returnTrigger;

        public void RegisterReference(GuestDataSO guestDataSo, IItemFactory itemFactory)
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            
            _guestStatModule = new GuestStatModule(guestDataSo);
            _guestMovementModule = new GuestMovementModule(this, _guestStatModule);
            _guestCollisionModule = new GuestCollisionModule(_guestStatModule);
            _guestInventoryModule = new GuestInventoryModule(transform, transform, _guestStatModule, itemFactory, CreatureType, NPCType);

            _guestCollisionModule.OnCompareWithTarget += HandleOnCompareWithTarget;
            _guestCollisionModule.OnTriggerTradeZone += HandleOnTriggerTradeZone;
            _guestCollisionModule.OnTriggerSpawnZone += HandleOnTriggerSpawnZone;
            _guestCollisionModule.OnTriggerPaymentZone += HandleOnTriggerPaymentZone;

            _guestInventoryModule.OnTargetQuantityReceived += CheckNextDestination;
        }

        public void Initialize(Vector3 startPoint, Action action)
        {
            _destinationIndex = 0;
            OnReturnGuest = action;
            
            SetActive(true);
            _guestMovementModule.Initialize(startPoint);
        }

        private void Update()
        {
            _guestInventoryModule.Update();
            
            if (_waitingTrigger)
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime >= _guestStatModule.WaitingTime)
                {
                    _returnTrigger = true;
                    _waitingTrigger = false;
                }
            }
            
            if (_returnTrigger) _guestMovementModule.SetDestination(_destinations.Last().Item2.position);
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
            Reset();
        }

        public void GetFromPool()
        {
            Reset();
        }

        public void ReturnToPool()
        {
            Reset();
        }
        
        private void Reset()
        {
            _elapsedTime = 0;
            _waitingTrigger = false;
            _returnTrigger = false;

            _guestInventoryModule.Reset();
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
                if (_destinationIndex == 0 || _destinationIndex == _destinations.Count - 1) _waitingTrigger = true;
                _guestMovementModule.ActivateNavMeshAgent(false);
                return true;
            }

            return false;
        }

        public void CheckNextDestination()
        {
            if (_waitingTrigger)
            {
                _waitingTrigger = false;
                _elapsedTime = 0f;
            }

            if (_destinationIndex < _destinations.Count - 1)
            {
                _destinationIndex++;
            }

            if (_destinationIndex == _destinations.Count - 1)
            {
                _returnTrigger = true;
            }
            
            _guestMovementModule.SetDestination(_destinations[_destinationIndex].Item2.position);
        }

        private void HandleOnTriggerSpawnZone()
        {
            if (_returnTrigger) OnReturnGuest?.Invoke();
        }
        
        private void HandleOnTriggerTradeZone(ITradeZone zone, bool isConnected)
        {
            _guestInventoryModule.RegisterItemReceiver(zone, isConnected); 
        }
            
        private void HandleOnTriggerPaymentZone(IPaymentZone zone, bool isConnected)
        {
            if (_destinationIndex != _destinations.Count - 1)
            {
                zone.RegisterPaymentTarget(this, true);   
            }
        }

        public Tuple<string, int> GetItem()
        {
            return _guestInventoryModule.GetItem();
        }
    }
}