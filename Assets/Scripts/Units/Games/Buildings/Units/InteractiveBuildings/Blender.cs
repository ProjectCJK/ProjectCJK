using System;
using Interfaces;
using ScriptableObjects.Scripts;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Games.Buildings.Abstract;
using Units.Games.Buildings.Enums;
using Units.Games.Buildings.Modules;
using Units.Games.Items.Controllers;
using Units.Games.Items.Enums;
using Units.Modules;
using Units.Modules.InventoryModules.Units;
using Units.Modules.StatsModules.Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Games.Buildings.Units.InteractiveBuildings
{
    public interface IBlender : IRegisterReference<BuildingDataSO, IItemController>
    {
        
    }
    
    public class Blender : Building, IBlender
    {
        public override EBuildingType BuildingType { get; protected set; }
        public override Tuple<EMaterialType, EProductType> InputItemKey { get; protected set; }
        public override Tuple<EMaterialType, EProductType> OutItemKey { get; protected set; }

        private IBuildingStatsModule _buildingStatsModule;
        private IBuildingInventoryModule _buildingInventoryModule;
        private IBuildingProductModule _buildingProductModule;
        private IItemController _itemController;
        private IInteractionTrade _interactionTrade;

        public void RegisterReference(BuildingDataSO buildingDataSo, IItemController itemController)
        {
            BuildingType = buildingDataSo.BuildingType;
            _itemController = itemController;
            
            var tilemap = GetComponent<Tilemap>();
            Vector3 senderPosition = tilemap.GetCellCenterWorld(tilemap.WorldToCell(transform.position));
            
            InputItemKey = new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.inputProductType);
            OutItemKey = new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.outputProductType);
            _buildingStatsModule = new BuildingStatsModule(buildingDataSo);
            _buildingInventoryModule = new BuildingInventoryModule(senderPosition, transform, _buildingStatsModule, _itemController, InputItemKey, OutItemKey);
            _buildingProductModule = new BuildingProductModule(_buildingStatsModule, _buildingInventoryModule, InputItemKey, OutItemKey);
            
            _interactionTrade = GetComponentInChildren<InteractionTrade>();
            _interactionTrade.RegisterReference(_buildingInventoryModule);
        }
        
        public override void Initialize() { }

        private void Update()
        {
            //TODO : Test Scripts
            if (Input.GetKeyDown(KeyCode.W))
            {
                _buildingInventoryModule.ReceiveItem(new Tuple<EMaterialType, EProductType>(EMaterialType.A, EProductType.Product));
            }
            
            _buildingInventoryModule.SendItem();
            _buildingProductModule.Product();
        }
    }
}