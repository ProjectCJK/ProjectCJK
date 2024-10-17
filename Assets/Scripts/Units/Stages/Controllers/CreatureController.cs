using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Creatures.Abstract;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface ICreatureController : IRegisterReference<Joystick, IItemController>, IInitializable
    {
        public void InstantiatePlayer();
        public Transform GetPlayerTransform();
    }
    
    public class CreatureController : MonoBehaviour, ICreatureController
    {
        [SerializeField] private PlayerDataSo _playerDataSo;
        // TODO: NPC Data So
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _customerSpawnPoint;
        
        private ICreatureFactory _playerFactory;
        private Creature _player;

        public void RegisterReference(Joystick joystick, IItemController itemController)
        {
            _playerFactory = new PlayerFactory(_playerDataSo, _playerSpawnPoint.position, joystick, itemController);
            _player = _playerFactory.CreateCreature();
        }
        
        public void Initialize()
        {
            _player.Initialize();
        }
        
        public void InstantiatePlayer()
        {
            
        }

        public Transform GetPlayerTransform() => _player.transform;
    }
}