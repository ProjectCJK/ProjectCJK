using System;
using System.Collections.Generic;
using AmplifyShaderEditor;
using Interfaces;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using EBuildingType = Units.Stages.Units.Buildings.Enums.EBuildingType;
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
                
                switch (building)
                {
                    case IKitchen kitchen:
                        kitchen.RegisterReference(itemController);
                        break;
                    case IStand stand:
                        stand.RegisterReference(itemController);
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