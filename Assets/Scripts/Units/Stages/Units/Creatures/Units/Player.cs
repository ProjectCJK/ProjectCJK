using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.BattleModules;
using Units.Modules.CollisionModules.Units;
using Units.Modules.FSMModules.Units;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IPlayer : IBaseCreature, IRegisterReference<PlayerDataSO, Joystick, IItemController>, IInitializable
    {
        public IItemReceiver PlayerInventoryModule { get; }
    }

    public class Player : Creature, IPlayer
    {
        [SerializeField] private Weapon _weapon;
     
        public IItemReceiver PlayerInventoryModule => _playerInventoryModule;
        
        public override ECreatureType CreatureType => _playerStatsModule.Type;
        public override Animator Animator { get; protected set; }
        public override Transform Transform => transform;

        protected override CreatureStateMachine creatureStateMachine { get; set; }
        
        private IPlayerInventoryModule _playerInventoryModule;
        private IPlayerStatsModule _playerStatsModule;
        private IPlayerBattleModule _playerBattleModule;
        private IPlayerMovementModule _playerMovementModule;
        private IPlayerCollisionModule _playerCollisionModule;
        private IItemController _itemController;
        private PlayerDataSO _playerDataSo;

        public void RegisterReference(PlayerDataSO playerDataSo, Joystick joystick, IItemController itemController)
        {
            _playerDataSo = playerDataSo;
            _itemController = itemController;
            
            Animator = spriteTransform.GetComponent<Animator>();

            creatureStateMachine = new CreatureStateMachine(this);
            _playerStatsModule = new PlayerStatsModule(_playerDataSo);
            _playerBattleModule = new PlayerBattleModule(_playerDataSo, joystick, transform, _weapon);
            _playerInventoryModule = new PlayerInventoryModule(transform, transform, _playerStatsModule, _itemController, CreatureType);
            _playerMovementModule = new PlayerMovementModule(this, _playerStatsModule, creatureStateMachine, joystick, spriteTransform);
            _playerCollisionModule = new PlayerCollisionModule();

            _weapon.RegisterReference(_playerStatsModule);
            
            _playerCollisionModule.OnTriggerTradeZone += HandleOnTriggerTradeZone;
            _playerCollisionModule.OnTriggerHuntingZone += HandleOnTriggerHuntingZone;
        }

        public void Initialize()
        {
            _playerMovementModule.Initialize();
            _playerInventoryModule.Initialize();
            _playerBattleModule.Initialize();
            
            creatureStateMachine.ChangeState(creatureStateMachine.CreatureIdleState);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var tempKey = new Tuple<EMaterialType, EItemType>(EMaterialType.A, EItemType.Material);
                _playerInventoryModule.ReceiveItemWithDestroy(tempKey, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
            }
            
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
        
        private void HandleOnTriggerTradeZone(IInteractionTrade interactionZone, bool isConnected)
        {
            _playerInventoryModule.ConnectWithInteractionTradeZone(interactionZone, isConnected);
        }
        
        private void HandleOnTriggerHuntingZone(bool value)
        {
            _playerBattleModule.HandleOnTriggerHuntingZone(value);
        }
    }
}