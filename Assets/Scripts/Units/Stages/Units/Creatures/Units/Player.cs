using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Modules;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules;
using Units.Modules.BattleModules;
using Units.Modules.CollisionModules.Units;
using Units.Modules.FactoryModules.Units;
using Units.Modules.FSMModules.Units;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units;
using Units.Modules.InventoryModules.Units.CreatureInventoryModules.Units;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Buildings.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.HuntingZones;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IPlayer : ICreature, IRegisterReference<PlayerDataSO, Joystick, IItemFactory>, IInitializable<Vector3, Action>
    {
        public IItemReceiver PlayerInventoryModule { get; }
        public float PaymentDelay { get; }
    }

    public class Player : Creature, IPlayer
    {
        private event Action OnReturnPlayer;
        [SerializeField] private Weapon _weapon;
     
        public IItemReceiver PlayerInventoryModule => _playerInventoryModule;
        public float PaymentDelay => _playerStatsModule.PaymentDelay;

        public override ECreatureType CreatureType => _playerStatsModule.CreatureType;
        public override Animator Animator => _animator;
        public override Transform Transform => transform;

        protected override CreatureStateMachine creatureStateMachine { get; set; }
        
        private IPlayerInventoryModule _playerInventoryModule;
        private IPlayerStatsModule _playerStatsModule;
        private IPlayerBattleModule _playerBattleModule;
        private IPlayerMovementModule _playerMovementModule;
        private IPlayerCollisionModule _playerCollisionModule;
        private IItemFactory _itemFactory;
        private IDamageFlashModule _damageFlashModule;
        
        private PlayerDataSO _playerDataSo;
        private Animator _animator;

        public void RegisterReference(PlayerDataSO playerDataSo, Joystick joystick, IItemFactory itemFactory)
        {
            _playerDataSo = playerDataSo;
            _itemFactory = itemFactory;
            
            _animator = spriteTransform.GetComponent<Animator>();
            _damageFlashModule = spriteTransform.GetComponent<DamageFlashModule>();
            
            creatureStateMachine = new CreatureStateMachine(this);
            _playerStatsModule = new PlayerStatsModule(_playerDataSo);
            _playerBattleModule = new PlayerBattleModule(joystick, transform, _weapon);
            _playerInventoryModule = new PlayerInventoryModule(transform, transform, _playerStatsModule, _itemFactory, CreatureType);
            _playerMovementModule = new PlayerMovementModule(this, _playerStatsModule, creatureStateMachine, joystick, spriteTransform);
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
            OnReturnPlayer = action;
            
            _playerMovementModule.Initialize();
            _playerInventoryModule.Initialize();
            _playerBattleModule.Initialize();
            
            creatureStateMachine.ChangeState(creatureStateMachine.CreatureIdleState);
            
            transform.position = position;
            transform.gameObject.SetActive(true);
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            // TODO : Cheat Code
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var temp1Key = EnumParserModule.ParseEnumToString(EItemType.Material, EMaterialType.A);
                var temp2Key = EnumParserModule.ParseEnumToString(EItemType.Material, EMaterialType.A);
                
                _playerInventoryModule.ReceiveItemThroughTransfer(temp1Key, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
                _playerInventoryModule.ReceiveItemThroughTransfer(temp2Key, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
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

        private void OnTriggerStay2D(Collider2D other)
        {
            _playerCollisionModule.OnTriggerStay2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _playerCollisionModule.OnTriggerExit2D(other);
        }
        
        private void HandleOnTriggerTradeZone(ITradeZone zone, bool isConnected)
        {
            _playerInventoryModule.RegisterItemReceiver(zone, isConnected);
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