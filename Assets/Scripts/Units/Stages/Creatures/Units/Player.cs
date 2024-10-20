using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Modules.DesignPatterns.FSMs.Modules;
using ScriptableObjects.Scripts.ScriptableObjects;
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
        public override ECreatureType CreatureType => _playerStatsModule.CreatureType;
        public override Animator Animator { get; protected set; }
        
        protected override CreatureStateMachine creatureStateMachine { get; set; }

        private PlayerDataSo _playerDataSo;
        
        private IPlayerStatsModule _playerStatsModule;
        private IPlayerMovementModule _playerMovementModule;
        private IPlayerInventoryModule _playerInventoryModule;
        private IPlayerCollisionModule _playerCollisionModule;
        private IItemController _itemController;

        public void RegisterReference(PlayerDataSo playerDataSo, Joystick joystick, IItemController itemController)
        {
            _playerDataSo = playerDataSo;
            _itemController = itemController;

            creatureStateMachine = new CreatureStateMachine(this);
            _playerStatsModule = new PlayerStatsModule(_playerDataSo);
            _playerInventoryModule = new PlayerInventoryModule(transform, transform, _playerStatsModule, _itemController, CreatureType);
            _playerMovementModule = new PlayerMovementModule(transform, _playerStatsModule, creatureStateMachine, joystick);
            _playerCollisionModule = new PlayerCollisionModule();
            
            Animator = spriteTransform.GetComponent<Animator>();
            
            _playerCollisionModule.OnTriggerInteractionZone += HandleInteractionZone;
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
        }

        private void FixedUpdate()
        {
            _playerMovementModule.Move();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            _playerCollisionModule.OnTriggerEnter2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _playerCollisionModule.OnTriggerExit2D(other);
        }
        
        private void HandleInteractionZone(Transform interactionZone, bool isConnected)
        {
            _playerInventoryModule.ConnectWithInteractionTradeZone(interactionZone, isConnected);
        }
    }
}