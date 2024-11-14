using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Units.Stages.Enums;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using EBuildingType = Units.Stages.Units.Buildings.Enums.EBuildingType;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    [Serializable]
    public struct BuildingSpawnData
    {
        public List<GameObject> KitchenPrefabs;
        public List<GameObject> StandPrefabs;
        public GameObject DeliveryLodgingPrefab;
        public GameObject ManagementDeskPrefab;
        public GameObject WareHousePrefab;
    }
    
    public interface IBuildingController : IRegisterReference<ItemFactory, List<EMaterialType>>, IInitializable
    {
        public Dictionary<string, BuildingZone> Buildings { get; }
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        [SerializeField] private BuildingSpawnData _buildingSpawnData;
        
        private List<EMaterialType> _currentActiveMaterials;

        private List<EMaterialType> _materials;
        public Dictionary<string, BuildingZone> Buildings { get; } = new();

        public void RegisterReference(ItemFactory itemFactory, List<EMaterialType> currentActiveMaterials)
        {
            _currentActiveMaterials = currentActiveMaterials;

            foreach (Transform buildingTransform in transform)
            {
                var building = buildingTransform.GetComponent<BuildingZone>();

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
                    case IDeliveryLodging deliveryLodging:
                        deliveryLodging.RegisterReference(itemFactory);
                        break;
                    case IWareHouse wareHouse:
                        wareHouse.RegisterReference(itemFactory);
                        break;
                }

                var key = building.BuildingKey;

                Buildings.TryAdd(key, building);

                if (building is UnlockableBuildingZone unlockableBuildingZone)
                {
                    VolatileDataManager.Instance.BuildingActiveStatuses.TryAdd(building, unlockableBuildingZone.ActiveStatus);

                    if (unlockableBuildingZone.ActiveStatus == EActiveStatus.Active)
                    {
                        var splits = building.BuildingKey.Split('_');

                        if (Enum.TryParse(splits[0], out EBuildingType buildingType) && splits.Length > 1)
                            if (Enum.TryParse(splits[1], out EMaterialType materialType) &&
                                buildingType == EBuildingType.Stand)
                                if (!_currentActiveMaterials.Contains(materialType))
                                    _currentActiveMaterials.Add(materialType);
                    }
                }
            }
        }

        public void Initialize()
        {
            foreach (KeyValuePair<string, BuildingZone> building in Buildings) building.Value.Initialize();
        }
    }
}