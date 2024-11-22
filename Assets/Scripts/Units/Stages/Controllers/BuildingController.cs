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
        [Header("=== Kitchen Spawner Position ===")]
        public List<Transform> KitchenSpawner;
        
        [Space(20), Header("=== Stand Spawner Position ===")]
        public List<Transform> StandSpawner;
        
        [Space(20), Header("=== ManagementDesk Position ===")]
        public Transform ManagementDeskSpawner;
        
        [Space(20), Header("=== DeliveryLodging Position ===")]
        public Transform DeliveryLodgingSpawner;
        
        [Space(20), Header("=== WareHouse Position ===")]
        public Transform WareHouseSpawner;
    }
    
    public interface IBuildingController : IRegisterReference<ItemFactory>, IInitializable
    {
        public Dictionary<string, BuildingZone> Buildings { get; }
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        [SerializeField] private BuildingSpawnData buildingSpawnData;
        public BuildingSpawnData BuildingSpawnData => buildingSpawnData;
        
        private List<EMaterialType> _currentActiveMaterials;
        public Dictionary<string, BuildingZone> Buildings { get; } = new();

        public void RegisterReference(ItemFactory itemFactory)
        {
            for (var i = 0; i < buildingSpawnData.KitchenSpawner.Count; i++)
            {
                InstantiateBuilding(buildingSpawnData.KitchenSpawner[i], DataManager.Instance.levelPrefabSo.Kitchen[i], itemFactory);
            }
            
            for (var i = 0; i < buildingSpawnData.StandSpawner.Count; i++)
            {
                InstantiateBuilding(buildingSpawnData.StandSpawner[i], DataManager.Instance.levelPrefabSo.Stand[i], itemFactory);
            }
            
            InstantiateBuilding(buildingSpawnData.ManagementDeskSpawner, DataManager.Instance.levelPrefabSo.ManagementDesk, itemFactory);
            InstantiateBuilding(buildingSpawnData.DeliveryLodgingSpawner, DataManager.Instance.levelPrefabSo.DeliveryLodging, itemFactory);
            InstantiateBuilding(buildingSpawnData.WareHouseSpawner, DataManager.Instance.levelPrefabSo.WareHouse, itemFactory);
        }

        private void InstantiateBuilding(Transform spawnTransform, GameObject prefab, ItemFactory itemFactory)
        {
            if (spawnTransform == null || prefab == null)
                return;

            GameObject instance = Instantiate(prefab, spawnTransform);
            instance.transform.localPosition = Vector3.zero;
            var building = instance.GetComponent<BuildingZone>();

            RegisterBuildingReferences(building, itemFactory);
        }

        private void RegisterBuildingReferences(BuildingZone building, ItemFactory itemFactory)
        {
            if (building == null)
                return;

            // Building 타입에 따른 참조 등록
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

            // if (building is UnlockableBuildingZone unlockableBuildingZone)
            // {
            //     VolatileDataManager.Instance.BuildingActiveStatuses.TryAdd(building, unlockableBuildingZone.ActiveStatus);
            //
            //     if (unlockableBuildingZone.ActiveStatus == EActiveStatus.Active)
            //     {
            //         var splits = building.BuildingKey.Split('_');
            //
            //         if (Enum.TryParse(splits[0], out EBuildingType buildingType) && splits.Length > 1)
            //         {
            //             if (Enum.TryParse(splits[1], out EMaterialType materialType) && buildingType == EBuildingType.Stand)
            //             {
            //                 if (!_currentActiveMaterials.Contains(materialType))
            //                     _currentActiveMaterials.Add(materialType);
            //             }
            //         }
            //     }
            // }
        }

        public void Initialize()
        {
            foreach (var building in Buildings.Values)
            {
                building.Initialize();
            }
        }
    }
}