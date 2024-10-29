using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Items;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IStageController : IRegisterReference<Joystick>, IInitializable
    {
        public Transform PlayerTransform { get; }
    }

    [Serializable]
    public struct StageReferences
    {
        public CreatureController CreatureController;
        public BuildingController BuildingController;
        public HuntingZoneController HuntingZoneController;
        public VillageZoneController VillageZoneController;
    }
    
    public class StageController : MonoBehaviour, IStageController
    {
        [Header("### Stage Settings ###")]
        public StageReferences stageReferences;
        
        [Space(10), SerializeField] private int maxGuestCount;

        public Transform PlayerTransform => _creatureController.PlayerTransform;
        
        private ICreatureController _creatureController => stageReferences.CreatureController;
        private IBuildingController _buildingController => stageReferences.BuildingController;
        private IHuntingZoneController _huntingZoneController => stageReferences.HuntingZoneController;
        private IVillageZoneController _villageZoneController => stageReferences.VillageZoneController;

        public void RegisterReference(Joystick joystick)
        {
            var itemFactory = new ItemFactory();
            
            _creatureController.RegisterReference(joystick, itemFactory);
            _buildingController.RegisterReference(itemFactory);
            _villageZoneController.RegisterReference(_creatureController, _buildingController, _huntingZoneController);
            _huntingZoneController.RegisterReference(_creatureController, itemFactory, _villageZoneController.Player);

            _villageZoneController.OnRegisterPlayer += _huntingZoneController.HandleOnRegisterPlayer;
        }
        
        public void Initialize()
        {
            _buildingController.Initialize();
            _huntingZoneController.Initialize();
            _villageZoneController.Initialize();
        }
    }
}