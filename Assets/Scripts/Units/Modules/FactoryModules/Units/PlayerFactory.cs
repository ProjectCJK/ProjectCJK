using Externals.Joystick.Scripts.Base;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
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
        public Player GetPlayer();
    }

    public class PlayerFactory : Factory, IPlayerFactory
    {
        public PlayerDataSO PlayerDataSo => DataManager.Instance.PlayerData;

        private readonly Joystick _joystick;
        private readonly IItemFactory _itemFactory;
        private readonly Vector3 _playerSpawnPoint;
        
        private Player _player;
        
        public PlayerFactory(Vector3 playerSpawnPoint, Joystick joystick, IItemFactory itemFactory)
        {
            _playerSpawnPoint = playerSpawnPoint;
            _joystick = joystick;
            _itemFactory = itemFactory;
            
            InstantiatePlayer();
        }
        
        public Player GetPlayer()
        {
            _player.Initialize();

            return _player;
        }

        private void InstantiatePlayer()
        {
            GameObject obj = Object.Instantiate(PlayerDataSo.prefab.gameObject, _playerSpawnPoint, Quaternion.identity);
            
            _player = obj.GetComponent<Player>();
            
            if (_player != null) _player.RegisterReference(PlayerDataSo, _joystick, _itemFactory);
        }
    }
}