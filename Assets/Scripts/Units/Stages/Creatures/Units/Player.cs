using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts.ScriptableObjects;
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
    public interface IPlayer : IRegisterReference<PlayerDataSo, Joystick, IItemController>, IRegisterEventListener
    {
        
    }

    public class Player : Creature, IPlayer
    {
        public override ECreatureType CreatureType => _playerStatsModule.CreatureType;
        
        private PlayerDataSo _playerDataSo;
        
        private IPlayerStatsModule _playerStatsModule;
        private IPlayerMovementModule _playerMovementModule;
        private IPlayerInventoryModule _playerInventoryModule;
        private IItemController _itemController;

        public void RegisterReference(PlayerDataSo playerDataSo, Joystick joystick, IItemController itemController)
        {
            _playerDataSo = playerDataSo;
            _itemController = itemController;
            
            _playerStatsModule = new PlayerStatsModule(_playerDataSo);
            _playerInventoryModule = new PlayerInventoryModule(transform, transform, _playerStatsModule, _itemController, CreatureType);
            _playerMovementModule = GetComponent<PlayerMovementModule>();
            _playerMovementModule.RegisterReference(_playerStatsModule, joystick);
        }
        
        public void RegisterEventListener()
        {
            _playerMovementModule.OnTriggerInteractionZone += _playerInventoryModule.ConnectWithInteractionTradeZone;
        }

        public override void Initialize()
        {
            _playerMovementModule.Initialize();
            _playerInventoryModule.Initialize();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _playerInventoryModule.ReceiveItem(new Tuple<EMaterialType, EProductType>(EMaterialType.A, EProductType.Material));
            }
            
            _playerInventoryModule.SendItem();
        }
    }
}