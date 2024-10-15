using System;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Games.Buildings.Enums;
using Units.Games.Buildings.Modules;
using Units.Games.Buildings.Units.Abstract;
using Units.Games.Items.Enums;
using Units.Modules;
using Units.Modules.InventoryModules.Units;
using Units.Modules.StatsModules.Units;
using UnityEngine;

namespace Units.Games.Buildings.Units.InteractiveBuildings
{
    public interface IShelf
    {
        
    }
    
    public class Shelf : InteractiveBuilding, IShelf
    {
        public override EBuildingType BuildingType { get; protected set; }
        public override Tuple<EMaterialType, EProductType> InputItemKey { get; protected set; }
        public override Tuple<EMaterialType, EProductType> OutItemKey { get; protected set; }
        
        private IBuildingStatsModule _buildingStatsModule;
        private IBuildingInventoryModule _buildingInventoryModule;
        private IInteractionTrade _interactionTrade;
        
        public void RegisterReference(BuildingDataSO buildingDataSo)
        {
            BuildingType = buildingDataSo.BuildingType;
            
            InputItemKey = new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.inputProductType);
            OutItemKey = new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.outputProductType);
            _buildingStatsModule = new BuildingStatsModule(buildingDataSo);
            // _buildingInventoryModule = new BuildingInventoryModule(_buildingStatsModule, InputItemKey, OutItemKey);
            
            _interactionTrade = GetComponentInChildren<InteractionTrade>();
            _interactionTrade.RegisterReference(_buildingInventoryModule);
        }
        
        public override void Initialize() { }

        private void Update()
        {
            _buildingInventoryModule.SendItem();
        }
    }
}