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
        public Player Player { get; }
    }
    
    public class StageController : MonoBehaviour, IStageController
    {
        [Header("### Stage Settings ###")]
        [SerializeField] private CreatureController _creatureController;
        [SerializeField] private BuildingController _buildingController;
        [SerializeField] private HuntingZoneController _huntingZoneController;
        
        public Player Player => _creatureController.Player;
        
        private IItemController _itemController;
        private IItemFactory _itemFactory;
        
        public void RegisterReference(Joystick joystick)
        {
            _itemController = new ItemController(new ItemFactory());
            
            _creatureController.RegisterReference(joystick, _itemController);
            _buildingController.RegisterReference(_itemController);
            _huntingZoneController.RegisterReference(_creatureController, _itemController, _creatureController.Player);
        }
        
        public void Initialize()
        {
            _creatureController.Initialize();
            _buildingController.Initialize();
            _huntingZoneController.Initialize();
        }
    }
}