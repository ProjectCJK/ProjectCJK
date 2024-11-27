using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.CollisionModules.Units;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Units;
using Units.Stages.Modules.MovementModules.Units;
using Units.Stages.Modules.SpriteModules;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IGuest : INPC, IPoolable, IRegisterReference<GuestDataSO, IItemFactory>,
        IInitializable<Vector3, Action, CreatureSprite>
    {
        public void SetDestinations(List<Tuple<string, Transform>> destinations);
        public void SetTargetPurchaseQuantity(int targetPurchaseQuantity);
        public void CheckNextDestination();
        public void Unregister();
    }

    public class Guest : NPC, IGuest
    {
        private CreatureSpriteModule _creatureSpriteModule;

        private CreatureStateMachine _creatureStateMachine;
        private int _destinationIndex;
        private List<Tuple<string, Transform, Action>> _destinations = new();
        private float _elapsedTime; // 시간 측정 변수
        private IGuestInventoryModule _guestInventoryModule;
        private IGuestMovementModule _guestMovementModule;
        private IGuestStatModule _guestStatModule;
        private bool _returnTrigger;
        private bool _waitingTrigger; // 대기 상태 여부
        public override Animator Animator { get; protected set; }

        private ITradeZone _currentTradeZone;
        private IPaymentZone _currentPaymentZone;

        private ENPCType NPCType => _guestStatModule.NPCType;

        private void Reset()
        {
            _elapsedTime = 0;
            _waitingTrigger = false;
            _returnTrigger = false;

            _guestInventoryModule.Reset();
            SetActive(false);
        }

        private void Update()
        {
            _guestInventoryModule.Update();
            _guestMovementModule.Update();

            if (_waitingTrigger)
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime >= _guestStatModule.WaitingTime)
                {
                    _returnTrigger = true;
                    _waitingTrigger = false;
                    HandleOnTriggerTradeZone(_currentTradeZone, false);
                }
            }

            if (_returnTrigger) _guestMovementModule.SetDestination(_destinations.Last());
        }

        private void FixedUpdate()
        {
            _guestMovementModule.FixedUpdate();
        }

        public override ECreatureType CreatureType => _guestStatModule.CreatureType;
        public override Transform Transform => transform;

        public void RegisterReference(GuestDataSO guestDataSo, IItemFactory itemFactory)
        {
            Animator = spriteTransform.GetComponent<Animator>();
            _creatureSpriteModule = spriteTransform.GetComponent<CreatureSpriteModule>();

            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            _creatureStateMachine = new CreatureStateMachine(this);
            _guestStatModule = new GuestStatModule(guestDataSo);
            _guestMovementModule =
                new GuestMovementModule(this, _guestStatModule, _creatureStateMachine, spriteTransform);
            _guestInventoryModule = new GuestInventoryModule(receiveTransform, receiveTransform, _guestStatModule, itemFactory,
                CreatureType, NPCType);

            _guestInventoryModule.OnTargetQuantityReceived += CheckNextDestination;
        }

        public void Initialize(Vector3 startPoint, Action action, CreatureSprite randomSprites)
        {
            _creatureSpriteModule.SetSprites(randomSprites);
            _destinationIndex = 0;
            OnReturnGuest = action;

            _creatureStateMachine.ChangeState(_creatureStateMachine.CreatureIdleState);

            SetActive(true);
            _guestMovementModule.Initialize(startPoint);
        }

        public void SetTargetPurchaseQuantity(int targetPurchaseQuantity)
        {
            _guestStatModule.SetMaxInventorySize(targetPurchaseQuantity);
        }

        public void SetDestinations(List<Tuple<string, Transform>> destinations)
        {
            _destinations.Clear();
            
            var currentTradeZoneTransform = destinations[0].Item2.GetComponent<BuildingZone>().TradeZoneNpcTransform;
            var currentPaymentZoneTransform = destinations[1].Item2.GetComponent<BuildingZone>().TradeZoneNpcTransform;

            _currentTradeZone = currentTradeZoneTransform.GetComponent<ITradeZone>();
            _currentPaymentZone = currentPaymentZoneTransform.GetComponent<IPaymentZone>();
            
            var newDestination = new List<Tuple<string, Transform, Action>>
            {
                new(destinations[0].Item1, currentTradeZoneTransform, () =>
                {
                    _waitingTrigger = true;
                    HandleOnTriggerTradeZone(_currentTradeZone, true);
                }),
                
                new(destinations[1].Item1, currentPaymentZoneTransform, () => HandleOnTriggerPaymentZone(_currentPaymentZone)),
                new(destinations[2].Item1, destinations[2].Item2, HandleOnTriggerSpawnZone)
            };


            _destinations = newDestination;
            _guestMovementModule.SetDestination(_destinations[_destinationIndex]);
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

        public void CheckNextDestination()
        {
            if (_waitingTrigger)
            {
                _waitingTrigger = false;
                _elapsedTime = 0f;
            }

            if (_destinationIndex == 0) HandleOnTriggerTradeZone(_currentTradeZone, false);

            if (_destinationIndex < _destinations.Count - 1) _destinationIndex++;

            if (_destinationIndex == _destinations.Count - 1) _returnTrigger = true;

            _guestMovementModule.SetDestination(_destinations[_destinationIndex]);
        }

        public void Unregister()
        {
            HandleOnTriggerTradeZone(_currentTradeZone, false);
        }

        private event Action OnReturnGuest;

        private void SetActive(bool value)
        {
            if (gameObject.activeInHierarchy != value) gameObject.SetActive(value);
        }

        private void HandleOnTriggerSpawnZone()
        {
            if (_returnTrigger) OnReturnGuest?.Invoke();
        }

        private void HandleOnTriggerTradeZone(ITradeZone zone, bool isConnected)
        {
            _guestInventoryModule.RegisterItemReceiver(zone, isConnected);
        }

        private void HandleOnTriggerPaymentZone(IPaymentZone zone)
        {
            if (_destinationIndex != _destinations.Count - 1) zone.RegisterPaymentTarget(this, true);
        }

        public Tuple<string, int> GetItem()
        {
            return _guestInventoryModule.GetItem();
        }
    }
}