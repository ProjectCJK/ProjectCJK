using System;
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
        
        public Player Player => _creatureController.GetPlayer();
        
        private IItemFactory _itemFactory;
        
        public void RegisterReference(Joystick joystick)
        {
            _itemFactory = new ItemFactory();
            
            _creatureController.RegisterReference(joystick, _itemFactory);
            _buildingController.RegisterReference(_itemFactory);
            _huntingZoneController.RegisterReference(_creatureController, _itemFactory, Player);
        }
        
        public void Initialize()
        {
            _buildingController.Initialize();
            _huntingZoneController.Initialize();
        }

        private void Update()
        {
#if UNITY_EDITOR
            // TODO : Cheat Code
            if (Input.GetKeyDown(KeyCode.E))
            {
                _creatureController.GetGuest(Player.transform);
            }      
#endif
        }
    }
}