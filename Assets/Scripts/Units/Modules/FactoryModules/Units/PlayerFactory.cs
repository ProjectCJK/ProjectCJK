using Externals.Joystick.Scripts.Base;
using Managers;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Controllers;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Modules.FactoryModules.Units
{
    public interface IPlayerFactory
    {
        public PlayerDataSO PlayerDataSo { get; }
        public Player CreatePlayer();
    }

    public class PlayerFactory : Factory, IPlayerFactory
    {
        public PlayerDataSO PlayerDataSo => DataManager.Instance.PlayerData;
        
        private readonly Joystick _joystick;
        private readonly IItemController _itemController;
        private readonly Vector3 _playerSpawnPoint;
        
        public PlayerFactory(Vector3 playerSpawnPoint, Joystick joystick, IItemController itemController)
        {
            _playerSpawnPoint = playerSpawnPoint;
            _joystick = joystick;
            _itemController = itemController;
        }

        public Player CreatePlayer()
        {
            GameObject obj = Object.Instantiate(PlayerDataSo.prefab.gameObject, _playerSpawnPoint, Quaternion.identity);
            var player = obj.GetComponent<Player>();
            
            if (player != null)
            {
                player.RegisterReference(PlayerDataSo, _joystick, _itemController);
            }
            
            return player;
        }
    }
}