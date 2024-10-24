using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Items;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IStageController : IRegisterReference<Joystick>, IInitializable
    {
        public IPlayer GetPlayer();
    }
    
    public class StageController : MonoBehaviour, IStageController
    {
        [Header("### Stage Settings ###")]
        [SerializeField] private CreatureController _creatureController;
        [SerializeField] private BuildingController _buildingController;
        [SerializeField] private HuntingZoneController _huntingZoneController;
        
        private ItemController _itemController;
        private IItemFactory _itemFactory;
        private IMonsterFactory _monsterFactory;
        
        public void RegisterReference(Joystick joystick)
        {
            _itemController = new ItemController(new ItemFactory());
            _monsterFactory = new MonsterFactory();
            
            _creatureController.RegisterReference(joystick, _itemController);
            _buildingController.RegisterReference(_itemController);
            _huntingZoneController.RegisterReference(_monsterFactory, _itemController, _creatureController.GetPlayer());
        }
        
        public void Initialize()
        {
            _creatureController.Initialize();
            _buildingController.Initialize();
            _huntingZoneController.Initialize();
        }

        public IPlayer GetPlayer() => _creatureController.GetPlayer();
    }
}