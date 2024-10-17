using System;
using System.Collections.Generic;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.InventoryModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Buildings.Abstract;
using Units.Stages.Buildings.Enums;
using Units.Stages.Buildings.Modules;
using Units.Stages.Controllers;
using Units.Stages.Items.Enums;

namespace Units.Stages.Buildings.Units
{
    public interface IShelf
    {
        
    }
    
    public class Shelf : Building, IShelf
    {
        public override EBuildingType BuildingType { get; protected set; }
        public override List<Tuple<EMaterialType, EProductType>> InputItemKey { get; protected set; }
        public override List<Tuple<EMaterialType, EProductType>> OutItemKey { get; protected set; }
        
        private IBuildingStatsModule _buildingStatsModule;
        private IShelfInventoryModule _shelfInventoryModule;
        private IItemController _itemController;
        private IInteractionTrade _interactionTrade;
        
        public void RegisterReference(BuildingDataSO buildingDataSo, IItemController itemController)
        {
            BuildingType = buildingDataSo.BuildingType;
            _itemController = itemController;
            
            InputItemKey = new List<Tuple<EMaterialType, EProductType>>();
            OutItemKey = new List<Tuple<EMaterialType, EProductType>>();
            
            InputItemKey.Add(new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.inputProductType));
            OutItemKey.Add(new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.outputProductType));
            
            _buildingStatsModule = new BuildingStatsModule(buildingDataSo);
            _shelfInventoryModule = new ShelfInventoryModule(transform, transform, _buildingStatsModule, _itemController, InputItemKey, OutItemKey);
            
            _interactionTrade = GetComponentInChildren<InteractionTrade>();
            _interactionTrade.RegisterReference(_shelfInventoryModule, InputItemKey);
        }
        
        public override void Initialize() { }

        private void Update()
        {
            _shelfInventoryModule.SendItem();
        }
    }
}