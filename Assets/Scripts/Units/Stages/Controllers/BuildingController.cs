using System;
using System.Collections.Generic;
using Interfaces;
using Units.Stages.Buildings.Abstract;
using Units.Stages.Buildings.Units;
using Units.Stages.Items.Enums;
using UnityEngine;
using EBuildingType = Units.Stages.Buildings.Enums.EBuildingType;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    public interface IBuildingController : IRegisterReference<IItemController>, IInitializable
    {
        
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        private readonly Dictionary<EBuildingType, Building> _buildings = new();
        private List<EMaterialType> _materials;

        public void RegisterReference(IItemController itemController)
        {
            foreach (Transform buildingTransform in transform)
            {
                var building = buildingTransform.GetComponent<Building>();
                
                switch (building.BuildingType)
                {
                    case EBuildingType.Kitchen:
                        if (building is IKichen blender) blender.RegisterReference(itemController);
                        break;
                    case EBuildingType.FoodStand:
                        if (building is IShelf shelf) shelf.RegisterReference(itemController);
                        break;
                }
                
                _buildings.TryAdd(building.BuildingType, building);
            }
        }

        public void Initialize()
        {
            foreach (KeyValuePair<EBuildingType, Building> building in _buildings)
            {
                building.Value.Initialize();
            }
        }
        
        public Transform GetBuildingTransform(EBuildingType key) => _buildings[key].transform;
    }
}