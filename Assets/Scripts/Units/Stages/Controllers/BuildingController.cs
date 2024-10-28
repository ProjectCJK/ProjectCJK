using System;
using System.Collections.Generic;
using AmplifyShaderEditor;
using Interfaces;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using EBuildingType = Units.Stages.Units.Buildings.Enums.EBuildingType;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    public interface IBuildingController : IRegisterReference<IItemFactory>, IInitializable
    {
        
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        private readonly Dictionary<EBuildingType, Building> _buildings = new();
        private List<EMaterialType> _materials;

        public void RegisterReference(IItemFactory itemFactory)
        {
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