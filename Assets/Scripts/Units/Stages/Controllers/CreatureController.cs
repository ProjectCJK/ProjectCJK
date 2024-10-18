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
        public Transform GetPlayerTransform();
    }
    
    public class CreatureController : MonoBehaviour, ICreatureController
    {
        [Header("=== 플레이어 세팅 ===")]
        [SerializeField] private PlayerDataSo _playerDataSo;
        [SerializeField] private Transform _playerSpawnPoint;
        
        [Header("=== 손님 NPC 세팅 ===")]
        // TODO: NPC Data So
        [SerializeField] private Transform _customerSpawnPoint;
        
        private ICreatureFactory _playerFactory;
        private Creature _player;

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
            _playerFactory = new PlayerFactory(_playerDataSo, _playerSpawnPoint.position, joystick, itemController);
            _player = _playerFactory.CreateCreature();
        }

        public Transform GetPlayerTransform() => _player.transform;
    }
}