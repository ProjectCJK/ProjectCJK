using System;
using System.Collections.Generic;
using Enums;
using Interfaces;
using Modules;
using ScriptableObjects.Scripts;
using Units.Games.Buildings.Abstract;
using Units.Games.Buildings.Enums;
using Units.Games.Creatures.Abstract;
using Units.Games.Creatures.Units;
using UnityEngine;
using EBuildingType = Units.Games.Buildings.Enums.EBuildingType;

namespace Units.Games.Buildings.Controllers
{
    public interface IBuildingController : IInitializable
    {
        public void SpawnBuilding();
    }

    public class BuildingController : IBuildingController
    {
        private readonly IBuildingFactory _buildingFactory;
        
        private Dictionary<Tuple<EBuildingType, EMaterialType>, Building> _buildings = new ();   
        private List<EMaterialType> _materials;

        public BuildingController(IBuildingFactory buildingFactory)
        {
            _buildingFactory = buildingFactory;
        }
        
        public void Initialize()
        {
            foreach (KeyValuePair<Tuple<EBuildingType, EMaterialType>, Building> building in _buildings)
            {
                building.Value.Initialize();
            }
        }

        public void SpawnBuilding()
        {
            _buildings = _buildingFactory.CreateBuilding();
        }

        public Transform GetBuildingTransform(Tuple<EBuildingType, EMaterialType> key) => _buildings[key].transform;
    }
}