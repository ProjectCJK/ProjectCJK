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
        private readonly IItemFactory _itemFactory;

        private readonly Joystick _joystick;

        private IPlayer _player;

        public PlayerFactory(Joystick joystick, IItemFactory itemFactory)
        {
            _joystick = joystick;
            _itemFactory = itemFactory;

            InstantiatePlayer();
        }

        public PlayerDataSO PlayerDataSo => DataManager.Instance.PlayerDataSo;
        public Transform PlayerTransform => _player.Transform;

        public IPlayer GetPlayer()
        {
            return _player;
        }

        private void InstantiatePlayer()
        {
            GameObject obj = Object.Instantiate(PlayerDataSo.prefab);

            _player = obj.GetComponent<IPlayer>();
            VolatileDataManager.Instance.Player = _player as Player;;
            _player.Transform.gameObject.SetActive(false);
            _player?.RegisterReference(PlayerDataSo, _joystick, _itemFactory);
        }
    }
}