using System;
using System.Collections.Generic;
using Interfaces;
using Units.Stages.Enums;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Units;
using UnityEngine;
using EBuildingType = Units.Stages.Units.Zones.Units.BuildingZones.Enums.EBuildingType;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    public interface IBuildingController : IRegisterReference<IItemFactory,List<EMaterialType>>, IInitializable
    {
        public Dictionary<string, BuildingZone> Buildings { get; }
        public Dictionary<BuildingZone, EActiveStatus> BuildingActiveStatuses { get; }
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        public Dictionary<string, BuildingZone> Buildings { get; } = new();
        public Dictionary<BuildingZone, EActiveStatus> BuildingActiveStatuses { get; } = new();

        private List<EMaterialType> _materials;
        private List<EMaterialType> _currentActiveMaterials;

        public void RegisterReference(IItemFactory itemFactory, List<EMaterialType> currentActiveMaterials)
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
                }

                var key = building.BuildingKey;

                Buildings.TryAdd(key, building);

                if (building is UnlockableBuildingZone unlockableBuildingZone)
                {
                    BuildingActiveStatuses.TryAdd(building, unlockableBuildingZone.ActiveStatus);

                    if (unlockableBuildingZone.ActiveStatus == EActiveStatus.Active)
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
        }

        public void Initialize()
        {
            foreach (KeyValuePair<string, BuildingZone> building in Buildings)
            {
                building.Value.Initialize();
            }
        }
    }
}