using System;
using System.Collections.Generic;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Buildings.Abstract;
using Units.Stages.Buildings.Enums;
using Units.Stages.Controllers;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Modules.FactoryModules.Units
{
    public interface IBuildingFactory
    {
        public Dictionary<Tuple<EBuildingType, EMaterialType>, Building> CreateBuilding();
    }

    public class BuildingFactory : Factory, IBuildingFactory
    {
        private readonly List<BuildingDataSO> _buildingStatSo;
        private readonly Grid _grid;
        private readonly IItemController _itemController;

        public BuildingFactory(List<BuildingDataSO> buildingStatSo, Grid grid, IItemController itemController)
        {
            _buildingStatSo = buildingStatSo;
            _grid = grid;
            _itemController = itemController;
        }
        
        public Dictionary<Tuple<EBuildingType, EMaterialType>, Building> CreateBuilding()
        {
            var buildings = new Dictionary<Tuple<EBuildingType, EMaterialType>, Building>();

            // foreach (BuildingDataSO buildingStatSo in _buildingStatSo)
            // {
            //     GameObject prefab = buildingStatSo.BaseSpawnData.Prefab;
            //     Vector3Int spawnPos = buildingStatSo.BaseSpawnData.Position;
            //
            //     GameObject obj = Object.Instantiate(prefab, _grid.transform, true);
            //     obj.transform.position = spawnPos;
            //     var building = obj.GetComponent<Building>();
            //     
            //     switch (building.BuildingType)
            //     {
            //         case EBuildingType.Blender:
            //             var blender = building as Blender;
            //             if (blender != null)
            //             {
            //                 blender.RegisterReference(buildingDataSo:buildingStatSo, itemController:_itemController);
            //             }
            //             buildings.TryAdd(new Tuple<EBuildingType, EMaterialType>(buildingStatSo.BuildingType, buildingStatSo.MaterialType), blender);
            //             break;
            //         case EBuildingType.Shelf:
            //             //TODO : 선반 Factory
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException();
            //     }
            // }

            return buildings;
        }
    }
}