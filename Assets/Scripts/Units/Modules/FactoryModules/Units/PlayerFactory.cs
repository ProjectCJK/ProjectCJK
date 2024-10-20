using Externals.Joystick.Scripts.Base;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Controllers;
using Units.Stages.Creatures.Abstract;
using Units.Stages.Creatures.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Modules.FactoryModules.Units
{
    public interface ICreatureFactory
    {
        Creature CreateCreature();
    }

    public class PlayerFactory : Factory, ICreatureFactory
    {
        private readonly PlayerDataSo _playerDataSo;
        private readonly Joystick _joystick;
        private readonly IItemController _itemController;
        private readonly Vector3 _playerSpawnPoint;
        
        public PlayerFactory(PlayerDataSo playerDataSo, Vector3 playerSpawnPoint, Joystick joystick, IItemController itemController)
        {
            _playerDataSo = playerDataSo;
            _playerSpawnPoint = playerSpawnPoint;
            _joystick = joystick;
            _itemController = itemController;
        }

        public Creature CreateCreature()
        {
            GameObject obj = Object.Instantiate(_playerDataSo.creaturePrefab, _playerSpawnPoint, Quaternion.identity);
            var baseCreature = obj.GetComponent<Creature>();
            
            var player = baseCreature as Player;
            
            if (player != null)
            {
                player.RegisterReference(_playerDataSo, _joystick, _itemController);
            }
            
            return baseCreature;
        }
    }
}