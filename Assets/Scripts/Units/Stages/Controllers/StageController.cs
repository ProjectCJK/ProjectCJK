using System;
using System.Collections.Generic;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Units.Modules.FactoryModules.Units;
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

    [Serializable]
    public struct StageDefaultSettings
    {
        public StageReferences stageReferences;
    }

    [Serializable]
    public struct MaterialMapping
    {
        public EMaterialType MaterialType;
        public EStageMaterialType StageMaterialType;
    }

    [Serializable]
    public struct StageCustomSettings
    {
        [Header("--- 재료 타입 정의 ---")]
        public List<MaterialMapping> materialMappings;
        
        [Header("최대 손님 수")] public int MaxGuestCount;
    }

    public class StageController : MonoBehaviour, IStageController
    {
        [Header("### Stage Default Settings ### "), SerializeField]
        private StageDefaultSettings _stageDefaultSettings;
        
        [Header("### Stage Custom Settings ### "), SerializeField, Space(10)]
        private StageCustomSettings _stageCustomSettings;

        public Transform PlayerTransform => _creatureController.PlayerTransform;
        
        private ICreatureController _creatureController => _stageDefaultSettings.stageReferences.CreatureController;
        private IBuildingController _buildingController => _stageDefaultSettings.stageReferences.BuildingController;
        private IHuntingZoneController _huntingZoneController => _stageDefaultSettings.stageReferences.HuntingZoneController;
        private IVillageZoneController _villageZoneController => _stageDefaultSettings.stageReferences.VillageZoneController;

        public void RegisterReference(Joystick joystick)
        {
            var itemFactory = new ItemFactory(transform, _stageCustomSettings.materialMappings);
            
            _creatureController.RegisterReference(joystick, itemFactory);
            _buildingController.RegisterReference(itemFactory);
            _villageZoneController.RegisterReference(_creatureController, _buildingController, _huntingZoneController, _stageCustomSettings);
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