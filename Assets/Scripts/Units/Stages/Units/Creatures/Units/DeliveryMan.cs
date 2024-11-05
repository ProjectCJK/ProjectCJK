using System;
using System.Collections.Generic;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.FSMModules.Units;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Units;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
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
    
    public interface IDeliveryMan : IPoolable, IRegisterReference<DeliveryManDataSO, IItemFactory>, IInitializable<Vector3>
    {
        public CommandState CommandState { get; set; }
        public void SetDestinations(Tuple<string, Transform> destinations);
        public bool IsInventoryFull();
        public bool HaveAnyItem();
        public Tuple<string, Transform> GetDestination();
    }
    
    public class DeliveryMan : NPC, IDeliveryMan
    {
        public CommandState CommandState { get; set; }
        
        public override ECreatureType CreatureType => _deliveryManStatsModule.CreatureType;
        public override Transform Transform => transform;
        public override Animator Animator { get; protected set; }
        
        // protected override CreatureStateMachine creatureStateMachine { get; set; }

        private IDeliveryManInventoryModule _deliveryManInventoryModule;
        private IDeliveryManStatsModule _deliveryManStatsModule;
        private IDeliveryManMovementModule _deliveryManMovementModule;
        private IDeliveryManCollisionModule _deliveryManCollisionModule;

        private Tuple<string, Transform> _destination;
        private ENPCType NPCType => _deliveryManStatsModule.NPCType;
        
        public void RegisterReference(DeliveryManDataSO deliveryManDataSo, IItemFactory itemFactory)
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            
            _deliveryManStatsModule = new DeliveryManStatsModule(deliveryManDataSo);
            _deliveryManMovementModule = new DeliveryManMovementModule(this, _deliveryManStatsModule);
            _deliveryManCollisionModule = new DeliveryManCollisionModule(_deliveryManStatsModule);
            _deliveryManInventoryModule = new DeliveryManInventoryModule(transform, transform, _deliveryManStatsModule, itemFactory, CreatureType, NPCType);
            
            _deliveryManCollisionModule.OnCompareWithTarget += HandleOnCompareWithTarget;
            _deliveryManCollisionModule.OnTriggerTradeZone += HandleOnTriggerTradeZone;
        }
        
        public void Initialize(Vector3 startPosition)
        {
            SetActive(true);
            CommandState = CommandState.NoOrder;
            _deliveryManMovementModule.Initialize(startPosition);
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
        
        private void Reset()
        {
            SetActive(false);
        }
        
        public bool IsInventoryFull() => _deliveryManInventoryModule.CurrentInventorySize >= _deliveryManInventoryModule.MaxInventorySize;
        public bool HaveAnyItem() => _deliveryManInventoryModule.CurrentInventorySize > 0;

        public Tuple<string, Transform> GetDestination() => _destination;

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