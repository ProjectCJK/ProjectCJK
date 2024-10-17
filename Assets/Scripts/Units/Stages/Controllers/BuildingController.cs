using System.Collections.Generic;
using Interfaces;
using Units.Stages.Buildings.Abstract;
using Units.Stages.Items.Enums;
using UnityEngine;
using EBuildingType = Units.Stages.Buildings.Enums.EBuildingType;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    public interface IBuildingController : IRegisterReference, IInitializable
    {
        
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        private readonly Dictionary<EBuildingType, Building> _buildings = new();
        private List<EMaterialType> _materials;

        public void RegisterReference()
        {
            foreach (Transform buildingTransform in transform)
            {
                var building = buildingTransform.GetComponent<Building>();
                _buildings.TryAdd(building.BuildingType, building);
            }
        }

        public void Initialize()
        {
            foreach (var building in _buildings)
            {
                building.Value.Initialize();
            }
        }
        
        public Transform GetBuildingTransform(EBuildingType key) => _buildings[key].transform;
    }
}