using System;
using Enums;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Games.Buildings.Abstract;
using Units.Games.Creatures.Abstract;
using Units.Games.Creatures.Enums;
using Units.Modules.InventoryModules;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units;
using UnityEngine;

namespace Units.Games.Creatures.Units
{
    public interface IPlayer : IRegisterReference<PlayerStatSO, Joystick>, IRegisterEventListener
    {
        
    }

    public class Player : Creature, IPlayer
    {
        public override ECreatureType CreatureType => _playerStatsModule.CreatureType;
        
        private PlayerStatSO _playerStatSo;
        
        private IPlayerStatsModule _playerStatsModule;
        private IPlayerMovementModule _playerMovementModule;
        private IPlayerInventoryModule _playerInventoryModule;

        public void RegisterReference(PlayerStatSO playerStatSo, Joystick joystick)
        {
            _playerStatSo = playerStatSo;
            
            _playerStatsModule = new PlayerStatsModule(_playerStatSo);
            _playerInventoryModule = new PlayerInventoryModule();
            
            _playerMovementModule = GetComponent<PlayerMovementModule>();
            
            _playerInventoryModule.RegisterReference(_playerStatsModule, CreatureType);
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
                Debug.Log($"{EMaterialType.A} {EItemType.Material} 추가!");
                _playerInventoryModule.ReceiveItem(new Tuple<EMaterialType, EItemType>(EMaterialType.A, EItemType.Material));
            }
            
            _playerInventoryModule.SendItem();
        }
    }
}