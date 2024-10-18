using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.InventoryModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Buildings.Abstract;
using Units.Stages.Buildings.Enums;
using Units.Stages.Buildings.Modules;
using Units.Stages.Buildings.UI.FoodStands;
using Units.Stages.Controllers;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Stages.Buildings.Units
{
    public interface IShelf : IRegisterReference<IItemController>
    {
        
    }
    
    public class FoodStand : Building, IShelf
    {
        [SerializeField] private FoodStandView foodStandView;
        
        public override List<Tuple<EMaterialType, EProductType>> InputItemKey { get; protected set; }
        public override List<Tuple<EMaterialType, EProductType>> OutItemKey { get; protected set; }
        
        private IBuildingStatsModule _buildingStatsModule;
        private IFoodStandInventoryModule _foodStandInventoryModule;
        private IItemController _itemController;
        private IInteractionTrade _interactionTrade;

        private FoodStandViewModel _foodStandViewModel;
        private FoodStandModel _foodStandModel;
        
        public void RegisterReference(IItemController itemController)
        {
            _itemController = itemController;
            
            InputItemKey = new List<Tuple<EMaterialType, EProductType>>();
            OutItemKey = new List<Tuple<EMaterialType, EProductType>>();
            
            InputItemKey.Add(new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.inputProductType));
            OutItemKey.Add(new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.outputProductType));
            
            _buildingStatsModule = new BuildingStatsModule(buildingDataSo);
            _foodStandInventoryModule = new FoodStandInventoryModule(transform, transform, _buildingStatsModule, _itemController, InputItemKey, OutItemKey);

            _foodStandModel = new FoodStandModel();
            _foodStandViewModel = new FoodStandViewModel(_foodStandModel);
            foodStandView.BindViewModel(_foodStandViewModel);
            
            _interactionTrade = GetComponentInChildren<InteractionTrade>();
            _interactionTrade.RegisterReference(_foodStandInventoryModule, InputItemKey);

            _foodStandInventoryModule.OnInventoryCountChanged += UpdateViewModel;
        }
        
        public override void Initialize() { }
        
        private void UpdateViewModel()
        {
            var remainedProductCount = _foodStandInventoryModule.GetItemCount(InputItemKey[0]);
            _foodStandViewModel.UpdateValues(remainedProductCount);
        }
    }
}