using Externals.Joystick.Scripts.Base;
using Managers;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Stages.Modules.FactoryModules.Units
{
    public interface IPlayerFactory
    {
        public PlayerDataSO PlayerDataSo { get; }
        public Transform PlayerTransform { get; }
        public IPlayer GetPlayer();
    }

    public class PlayerFactory : Factory, IPlayerFactory
    {
        public PlayerDataSO PlayerDataSo => DataManager.Instance.PlayerDataSo;
        public Transform PlayerTransform => _player.Transform;

        private readonly Joystick _joystick;
        private readonly IItemFactory _itemFactory;
        
        private IPlayer _player;
        
        public PlayerFactory(Joystick joystick, IItemFactory itemFactory)
        {
            _joystick = joystick;
            _itemFactory = itemFactory;
            
            InstantiatePlayer();
        }
        
        public IPlayer GetPlayer()
        {
            return _player;
        }

        private void InstantiatePlayer()
        {
            GameObject obj = Object.Instantiate(PlayerDataSo.prefab);
            
            _player = obj.GetComponent<IPlayer>();
            _player.Transform.gameObject.SetActive(false);
            _player?.RegisterReference(PlayerDataSo, _joystick, _itemFactory);
        }
    }
}