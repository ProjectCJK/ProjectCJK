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
        public Player Player { get; }
    }

    [Serializable]
    public struct StageReferences
    {
        public CreatureController CreatureController;
        public BuildingController BuildingController;
        public HuntingZoneController HuntingZoneController;
    }
    
    public class StageController : MonoBehaviour, IStageController
    {
        [Header("### Stage Settings ###")]
        public StageReferences stageReferences;
        
        [Space(10), SerializeField] private int maxGuestCount;
        
        public Player Player => _creatureController.GetPlayer();
        
        private ICreatureController _creatureController => stageReferences.CreatureController;
        private IBuildingController _buildingController => stageReferences.BuildingController;
        private IHuntingZoneController _huntingZoneController => stageReferences.HuntingZoneController;

        public void RegisterReference(Joystick joystick)
        {
            var itemFactory = new ItemFactory();
            
            _creatureController.RegisterReference(joystick, itemFactory);
            _buildingController.RegisterReference(itemFactory);
            _huntingZoneController.RegisterReference(_creatureController, itemFactory, Player);
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
                _creatureController.GetGuest(_buildingController.Buildings[new Tuple<EBuildingType, EMaterialType>(EBuildingType.Stand, EMaterialType.A)].transform);
            }
#endif
        }
    }
}