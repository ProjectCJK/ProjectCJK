using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules;
using Units.Stages.Modules.BattleModules;
using Units.Stages.Modules.BattleModules.Units;
using Units.Stages.Modules.CollisionModules.Units;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Units;
using Units.Stages.Modules.MovementModules.Units;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Buildings.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IPlayer : ICreature, IRegisterReference<PlayerDataSO, Joystick, IItemFactory>,
        IInitializable<Vector3, Action>
    {
        public IItemReceiver PlayerInventoryModule { get; }
        public float PaymentDelay { get; }
    }

    public class Player : Creature, IPlayer
    {
        // private event Action OnReturnPlayer;

        [SerializeField] private Weapon _weapon;

        private CreatureStateMachine _creatureStateMachine;
        private IDamageFlashModule _damageFlashModule;
        private IItemFactory _itemFactory;
        private IPlayerBattleModule _playerBattleModule;
        private IPlayerCollisionModule _playerCollisionModule;

        private PlayerDataSO _playerDataSo;

        private IPlayerInventoryModule _playerInventoryModule;
        private IPlayerMovementModule _playerMovementModule;
        private IPlayerStatsModule _playerStatsModule;
        public override Animator Animator { get; protected set; }
        public ICreatureItemReceiver CreatureItemReceiver => _playerInventoryModule;

        private void Update()
        {
#if UNITY_EDITOR
            // TODO : Cheat Code
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var temp1Key = ParserModule.ParseEnumToString(EItemType.Material, EMaterialType.A);
                var temp2Key = ParserModule.ParseEnumToString(EItemType.Material, EMaterialType.B);

                _playerInventoryModule.ReceiveItemThroughTransfer(temp1Key, 1,
                    new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
                _playerInventoryModule.ReceiveItemThroughTransfer(temp2Key, 1,
                    new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
            }
#endif

            _playerInventoryModule.Update();
            _playerMovementModule.Update();
            _playerBattleModule.Update();
            _playerCollisionModule.Update();
        }

        private void FixedUpdate()
        {
            _playerMovementModule.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _playerCollisionModule.OnTriggerEnter2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _playerCollisionModule.OnTriggerExit2D(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            _playerCollisionModule.OnTriggerStay2D(other);
        }

        public IItemReceiver PlayerInventoryModule => _playerInventoryModule;
        public float PaymentDelay => _playerStatsModule.PaymentDelay;

        public override ECreatureType CreatureType => _playerStatsModule.CreatureType;
        public override Transform Transform => transform;

        public void RegisterReference(PlayerDataSO playerDataSo, Joystick joystick, IItemFactory itemFactory)
        {
            Animator = spriteTransform.GetComponent<Animator>();
            _damageFlashModule = spriteTransform.GetComponent<DamageFlashModule>();

            _playerDataSo = playerDataSo;
            _itemFactory = itemFactory;

            _creatureStateMachine = new CreatureStateMachine(this);
            _playerStatsModule = new PlayerStatsModule(_playerDataSo);
            _playerBattleModule = new PlayerBattleModule(joystick, transform, _weapon);
            _playerInventoryModule =
                new PlayerInventoryModule(transform, transform, _playerStatsModule, _itemFactory, CreatureType);
            _playerMovementModule = new PlayerMovementModule(this, _playerStatsModule, _creatureStateMachine, joystick,
                spriteTransform);
            _playerCollisionModule = new PlayerCollisionModule(_playerStatsModule);

            _damageFlashModule.RegisterReference();
            _weapon.RegisterReference(_playerStatsModule);

            _playerCollisionModule.OnTriggerTradeZone += HandleOnTriggerTradeZone;
            _playerCollisionModule.OnTriggerHuntingZone += HandleOnTriggerHuntingZone;
            _playerCollisionModule.OnTriggerPaymentZone += HandleOnTriggerPaymentZone;
            _weapon.AttackTrigger.OnHitSuccessful += _playerMovementModule.HandleOnHit;
        }

        public void Initialize(Vector3 position, Action action)
        {
            // OnReturnPlayer = action;

            _playerMovementModule.Initialize();
            _playerInventoryModule.Initialize();
            _playerBattleModule.Initialize();

            _creatureStateMachine.ChangeState(_creatureStateMachine.CreatureIdleState);

            transform.position = position;
            transform.gameObject.SetActive(true);
        }

        private void HandleOnTriggerTradeZone(ITradeZone zone, bool value)
        {
            _playerInventoryModule.RegisterItemReceiver(zone, value);
        }

        private void HandleOnTriggerHuntingZone(bool value)
        {
            _playerBattleModule.HandleOnTriggerHuntingZone(value);
        }

        private void HandleOnTriggerPaymentZone(IPaymentZone zone, bool value)
        {
            zone.RegisterPaymentTarget(this, value);
        }
    }
}