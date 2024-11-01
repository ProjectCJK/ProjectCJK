using System;
using System.Collections.Generic;
using AmplifyShaderEditor;
using Interfaces;
using Modules.DesignPatterns.FSMs.Enums;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Enums;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using EBuildingType = Units.Stages.Units.Buildings.Enums.EBuildingType;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    public interface IBuildingController : IRegisterReference<IItemFactory,List<EMaterialType>>, IInitializable
    {
        public Dictionary<string, Building> Buildings { get; }
        public Dictionary<Building, EActiveStatus> BuildingActiveStatuses { get; }
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        public Dictionary<string, Building> Buildings { get; } = new();
        public Dictionary<Building, EActiveStatus> BuildingActiveStatuses { get; } = new();

        private List<EMaterialType> _materials;
        private List<EMaterialType> _currentActiveMaterials;

        public void RegisterReference(IItemFactory itemFactory, List<EMaterialType> currentActiveMaterials)
        {
            _currentActiveMaterials = currentActiveMaterials;
            
            foreach (Transform buildingTransform in transform)
            {
                var building = buildingTransform.GetComponent<Building>();
                
                switch (building)
                {
                    case IKitchen kitchen:
                        kitchen.RegisterReference(itemFactory);
                        break;
                    case IStand stand:
                        stand.RegisterReference(itemFactory);
                        break;
                    case IManagementDesk managementDesk:
                        managementDesk.RegisterReference(itemFactory);
                        break;
                }

                var key = building.BuildingKey;

                Buildings.TryAdd(key, building);
                
                //TODO : 이후 건물 해금 기능 추가 시 이 부분 수정해야 함
                var tempActiveStatus = EActiveStatus.Active;
                BuildingActiveStatuses.TryAdd(building, tempActiveStatus);

                if (tempActiveStatus == EActiveStatus.Active)
                {
                    var splits = building.BuildingKey.Split('_');

                    if (Enum.TryParse(splits[0], out EBuildingType buildingType) && splits.Length > 1)
                    {
                        if (Enum.TryParse(splits[1], out EMaterialType materialType) && buildingType == EBuildingType.Stand)
                        {
                            if (!_currentActiveMaterials.Contains(materialType))
                            {
                                _currentActiveMaterials.Add(materialType);
                            }
                        }
                    }
                }
            }
        }

        public void Initialize()
        {
            foreach (KeyValuePair<string, Building> building in Buildings)
            {
                building.Value.Initialize();
            }
        }
        
        public Transform GetBuildingTransform(string key) => Buildings[key].transform;
    }
}