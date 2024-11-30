using System;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.BattleModules;
using Units.Stages.Modules.BattleModules.Units;
using Units.Stages.Modules.CollisionModules.Units;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.InventoryModules.Interfaces;
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
    public interface IHunter : IPoolable, ICreature, IRegisterReference<HunterDataSO, IItemFactory>,
        IInitializable<Vector3, CreatureSprite>
    {
        public CommandState CommandState { get; set; }
        public IItemReceiver HunterInventoryModule { get; }
        public void SetDestinations(Tuple<string, Transform> destination);
        public bool IsInventoryFull();
        public bool HaveAnyItem();
        public Tuple<string, Transform> GetDestination();
        public void SetMovementSpeed(float currentDeliveryLodgingOption1Value);
        public void InactivateWeapon();
    }

    public class Hunter : NPC, IHunter
    {
        [SerializeField] private Weapon _weapon;
        private CreatureSpriteModule _creatureSpriteModule;

        private CreatureStateMachine _creatureStateMachine;

        private Tuple<string, Transform> _destination;
        private HunterBattleModule _hunterBattleModule;
        private IHunterCollisionModule _hunterCollisionModule;

        private IHunterInventoryModule _hunterInventoryModule;
        private IHunterMovementModule _hunterMovementModule;
        private IHunterStatsModule _hunterStatsModule;
        public override Animator Animator { get; protected set; }
        private ENPCType NPCType => _hunterStatsModule.NPCType;

        private void Reset()
        {
            SetActive(false);
        }

        private void Update()
        {
            _hunterMovementModule.Update();
            _hunterInventoryModule.Update();
            _hunterBattleModule.Update();
        }

        private void FixedUpdate()
        {
            _hunterMovementModule.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _hunterCollisionModule.OnTriggerEnter2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _hunterCollisionModule.OnTriggerExit2D(other);
        }

        public IItemReceiver HunterInventoryModule => _hunterInventoryModule;
        public CommandState CommandState { get; set; }

        public override ECreatureType CreatureType => _hunterStatsModule.CreatureType;
        public override Transform Transform => transform;

        public void RegisterReference(HunterDataSO hunterDataSo, IItemFactory itemFactory)
        {
            Animator = spriteTransform.GetComponent<Animator>();
            _creatureSpriteModule = spriteTransform.GetComponent<CreatureSpriteModule>();

            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            _creatureStateMachine = new CreatureStateMachine(this);
            _hunterStatsModule = new HunterStatsModule(hunterDataSo);
            _hunterMovementModule =
                new HunterMovementModule(this, _hunterStatsModule, _creatureStateMachine, spriteTransform);
            _hunterBattleModule = new HunterBattleModule(_hunterMovementModule, transform, _weapon);
            _hunterCollisionModule = new HunterCollisionModule(_hunterStatsModule);
            _hunterInventoryModule = new HunterInventoryModule(transform, transform, _hunterStatsModule, itemFactory,
                CreatureType, NPCType);

            _weapon.RegisterReference(_hunterStatsModule);

            _hunterCollisionModule.OnCompareWithTarget += HandleOnCompareWithTarget;
            _hunterCollisionModule.OnTriggerTradeZone += HandleOnTriggerTradeZone;
            _hunterCollisionModule.OnTriggerHuntingZone += HandleOnTriggerHuntingZone;
            _weapon.AttackTrigger.OnHitSuccessful += _hunterMovementModule.HandleOnHit;
        }

        public void Initialize(Vector3 startPosition, CreatureSprite randomSprites)
        {
            _creatureSpriteModule.SetSprites(randomSprites);

            SetActive(true);
            CommandState = CommandState.NoOrder;
            _hunterMovementModule.Initialize(startPosition);
            _hunterBattleModule.Initialize();
        }

        public void SetDestinations(Tuple<string, Transform> destination)
        {
            _destination = destination;
            _hunterMovementModule.SetDestination(_destination);
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
            return _hunterInventoryModule.CurrentInventorySize >= _hunterInventoryModule.MaxInventorySize;
        }

        public bool HaveAnyItem()
        {
            return _hunterInventoryModule.CurrentInventorySize > 0;
        }

        public Tuple<string, Transform> GetDestination()
        {
            return _destination;
        }

        public void SetMovementSpeed(float currentDeliveryLodgingOption1Value)
        {
            _hunterStatsModule.MovementSpeed = currentDeliveryLodgingOption1Value;
        }

        public void InactivateWeapon()
        {
            _hunterBattleModule.ActivateWeapon(false);
        }

        private void SetActive(bool value)
        {
            if (gameObject.activeInHierarchy != value) gameObject.SetActive(value);
        }

        private bool HandleOnCompareWithTarget(string buildingKey)
        {
            if (_destination != null && string.Equals(buildingKey, _destination.Item1))
            {
                _hunterMovementModule.ActivateNavMeshAgent(false);
                return true;
            }

            return false;
        }

        private void HandleOnTriggerTradeZone(ITradeZone zone, bool isConnected)
        {
            _hunterInventoryModule.RegisterItemReceiver(zone, isConnected);
        }

        private void HandleOnTriggerHuntingZone(bool value)
        {
            _hunterBattleModule.HandleOnTriggerHuntingZone(value);
        }
    }
}