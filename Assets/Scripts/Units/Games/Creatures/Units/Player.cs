using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Games.Buildings.Abstract;
using Units.Games.Creatures.Abstract;
using Units.Games.Creatures.Enums;
using Units.Games.Items.Controllers;
using Units.Games.Items.Enums;
using Units.Modules.InventoryModules;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units;
using UnityEngine;

namespace Units.Games.Creatures.Units
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