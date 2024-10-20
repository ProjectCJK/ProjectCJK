using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Modules.DesignPatterns.FSMs.Modules;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.BattleModules;
using Units.Modules.CollisionModules;
using Units.Modules.FSMModules.Units;
using Units.Modules.InventoryModules.Units;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Controllers;
using Units.Stages.Creatures.Abstract;
using Units.Stages.Creatures.Enums;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Stages.Creatures.Units
{
    public interface IPlayer : IRegisterReference<PlayerDataSo, Joystick, IItemController>
    {
        
    }

    public class Player : Creature, IPlayer
    {
        [SerializeField] private Transform _weaponTransform;
        
        public override ECreatureType CreatureType => _playerStatsModule.CreatureType;
        public override Animator Animator { get; protected set; }
        
        protected override CreatureStateMachine creatureStateMachine { get; set; }

        private PlayerDataSo _playerDataSo;
        
        private IPlayerStatsModule _playerStatsModule;
        private IPlayerBattleModule _playerBattleModule;
        private IPlayerMovementModule _playerMovementModule;
        private IPlayerInventoryModule _playerInventoryModule;
        private IPlayerCollisionModule _playerCollisionModule;
        private IItemController _itemController;

        public void RegisterReference(PlayerDataSo playerDataSo, Joystick joystick, IItemController itemController)
        {
            Transform playerTransform = transform;
            
            _playerDataSo = playerDataSo;
            _itemController = itemController;
            
            var rigidBody2D = GetComponent<Rigidbody2D>();
            Animator = spriteTransform.GetComponent<Animator>();

            creatureStateMachine = new CreatureStateMachine(this);
            _playerStatsModule = new PlayerStatsModule(_playerDataSo);
            _playerBattleModule = new PlayerBattleModule(joystick, playerTransform, _weaponTransform);
            _playerInventoryModule = new PlayerInventoryModule(playerTransform, playerTransform, _playerStatsModule, _itemController, CreatureType);
            _playerMovementModule = new PlayerMovementModule(_playerStatsModule, creatureStateMachine, joystick, rigidBody2D, spriteTransform);
            _playerCollisionModule = new PlayerCollisionModule();
            
            _playerCollisionModule.OnTriggerTradeZone += HandleOnTriggerTradeZone;
            _playerCollisionModule.OnTriggerHuntingZone += HandleOnTriggerHuntingZone;
        }

        public override void Initialize()
        {
            _playerMovementModule.Initialize();
            _playerInventoryModule.Initialize();
            creatureStateMachine.ChangeState(creatureStateMachine.CreatureIdleState);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _playerInventoryModule.ReceiveItem(new Tuple<EMaterialType, EItemType>(EMaterialType.A, EItemType.Material));
            }
            
            _playerInventoryModule.SendItem();
            _playerMovementModule.FlipSprite();
            _playerBattleModule.RotateWeapon();
        }

        private void FixedUpdate()
        {
            _playerMovementModule.Move();
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
        
        private void HandleOnTriggerTradeZone(Transform interactionZone, bool isConnected)
        {
            _playerInventoryModule.ConnectWithInteractionTradeZone(interactionZone, isConnected);
        }
        
        private void HandleOnTriggerHuntingZone(bool value)
        {
            _playerBattleModule.HandleOnTriggerHuntingZone(value);
        }
    }
}