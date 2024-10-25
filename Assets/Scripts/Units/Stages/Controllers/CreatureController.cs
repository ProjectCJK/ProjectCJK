using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface ICreatureController : IRegisterReference<Joystick, IItemController>, IInitializable
    {
        public IPlayer GetPlayer();
    }
    
    public class CreatureController : MonoBehaviour, ICreatureController
    {
        [Header("=== 플레이어 세팅 ===")]
        [SerializeField] private Transform _playerSpawnPoint;
        
        [Header("=== 손님 NPC 세팅 ===")]
        // TODO: NPC Data So
        [SerializeField] private Transform _customerSpawnPoint;
        
        private IPlayerFactory _playerFactory;
        // private IGuestFactory _guestFactory;
        private Player _player;

        public void RegisterReference(Joystick joystick, IItemController itemController)
        {
            InstantiatePlayer(joystick, itemController);
        }
        
        public void Initialize()
        {
            _player.Initialize();
        }
        
        private void InstantiatePlayer(Joystick joystick, IItemController itemController)
        {
            _playerFactory = new PlayerFactory(_playerSpawnPoint.position, joystick, itemController);
            _player = _playerFactory.CreatePlayer();
        }

        public IPlayer GetPlayer() => _player;
    }
}