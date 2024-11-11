using System;
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
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Stages.Units.Creatures.Units
{
    public enum CommandState
    {
        Standby,
        NoOrder,
        MoveTo,
        Deliver
    }

    public interface IDeliveryMan : IPoolable, IRegisterReference<DeliveryManDataSO, IItemFactory>,
        IInitializable<Vector3, CreatureSprite>
    {
        public CommandState CommandState { get; set; }
        public void SetDestinations(Tuple<string, Transform> destinations);
        public bool IsInventoryFull();
        public bool HaveAnyItem();
        public Tuple<string, Transform> GetDestination();
        public void SetMovementSpeed(float currentDeliveryLodgingOption1Value);
    }

    public class DeliveryMan : NPC, IDeliveryMan
    {
        private CreatureSpriteModule _creatureSpriteModule;

        private CreatureStateMachine _creatureStateMachine;
        private IDeliveryManCollisionModule _deliveryManCollisionModule;

        private IDeliveryManInventoryModule _deliveryManInventoryModule;
        private IDeliveryManMovementModule _deliveryManMovementModule;
        private IDeliveryManStatsModule _deliveryManStatsModule;

        private Tuple<string, Transform> _destination;

        public override ECreatureType CreatureType => _deliveryManStatsModule.CreatureType;
        public override Transform Transform => transform;
        public override Animator Animator { get; protected set; }
        private ENPCType NPCType => _deliveryManStatsModule.NPCType;

        private void Reset()
        {
            SetActive(false);
        }

        private void Update()
        {
            _deliveryManMovementModule.Update();
            _deliveryManInventoryModule.Update();
        }

        private void FixedUpdate()
        {
            _deliveryManMovementModule.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _deliveryManCollisionModule.OnTriggerEnter2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _deliveryManCollisionModule.OnTriggerExit2D(other);
        }

        public CommandState CommandState { get; set; }

        public void RegisterReference(DeliveryManDataSO deliveryManDataSo, IItemFactory itemFactory)
        {
            Animator = spriteTransform.GetComponent<Animator>();
            _creatureSpriteModule = spriteTransform.GetComponent<CreatureSpriteModule>();

            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            _creatureStateMachine = new CreatureStateMachine(this);
            _deliveryManStatsModule = new DeliveryManStatsModule(deliveryManDataSo);
            _deliveryManMovementModule = new DeliveryManMovementModule(this, _deliveryManStatsModule,
                _creatureStateMachine, spriteTransform);
            _deliveryManCollisionModule = new DeliveryManCollisionModule(_deliveryManStatsModule);
            _deliveryManInventoryModule = new DeliveryManInventoryModule(transform, transform, _deliveryManStatsModule,
                itemFactory, CreatureType, NPCType);

            _deliveryManCollisionModule.OnCompareWithTarget += HandleOnCompareWithTarget;
            _deliveryManCollisionModule.OnTriggerTradeZone += HandleOnTriggerTradeZone;
        }

        public void Initialize(Vector3 startPosition, CreatureSprite randomSprites)
        {
            _creatureSpriteModule.SetSprites(randomSprites);
            SetActive(true);
            CommandState = CommandState.NoOrder;
            _deliveryManMovementModule.Initialize(startPosition);
        }

        public void SetDestinations(Tuple<string, Transform> destination)
        {
            if (Equals(destination, _destination)) return;

            _destination = destination;
            _deliveryManMovementModule.SetDestination(destination.Item2.position);
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

        public bool IsInventoryFull()
        {
            return _deliveryManInventoryModule.CurrentInventorySize >= _deliveryManInventoryModule.MaxInventorySize;
        }

        public bool HaveAnyItem()
        {
            return _deliveryManInventoryModule.CurrentInventorySize > 0;
        }

        public Tuple<string, Transform> GetDestination()
        {
            return _destination;
        }

        public void SetMovementSpeed(float currentDeliveryLodgingOption1Value)
        {
            _deliveryManStatsModule.MovementSpeed = currentDeliveryLodgingOption1Value;
        }

        private void SetActive(bool value)
        {
            if (gameObject.activeInHierarchy != value) gameObject.SetActive(value);
        }

        private bool HandleOnCompareWithTarget(string buildingKey)
        {
            if (string.Equals(buildingKey, _destination.Item1))
            {
                _deliveryManMovementModule.ActivateNavMeshAgent(false);
                return true;
            }

            return false;
        }

        private void HandleOnTriggerTradeZone(ITradeZone zone, bool isConnected)
        {
            _deliveryManInventoryModule.RegisterItemReceiver(zone, isConnected);
        }
    }
}