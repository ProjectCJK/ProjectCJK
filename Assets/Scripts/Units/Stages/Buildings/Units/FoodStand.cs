using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.InventoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Buildings.Abstract;
using Units.Stages.Buildings.Enums;
using Units.Stages.Buildings.Modules;
using Units.Stages.Buildings.UI.FoodStands;
using Units.Stages.Buildings.UI.FoodStands;
using Units.Stages.Controllers;
using Units.Stages.Items.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Stages.Buildings.Units
{
    public interface IFoodStand : IRegisterReference<IItemController>
    {
        
    }
    
    [Serializable]
    public struct FoodStandDefaultSetting
    {
        [Header("DataSo")]
        public BuildingDataSO BuildingDataSo;
        
        [Header("FoodStand UI")]
        public FoodStandView FoodStandView;
    }
    
    [Serializable]
    public struct FoodStandCustomSetting
    {
        [Header("재료 타입")]
        public EMaterialType MaterialType;
        
        [Header("Input 아이템 타입")]
        public EItemType InputItemType;
        
        [Header("Output 아이템 타입")]
        public EItemType OutputItemType;
    }
    
    public class FoodStand : Building, IFoodStand
    {
        public override EBuildingType BuildingType => FoodStandDefaultSetting.BuildingDataSo.BuildingType;
        public override List<Tuple<EMaterialType, EItemType>> InputItemKey { get; protected set; }
        public override List<Tuple<EMaterialType, EItemType>> OutItemKey { get; protected set; }

        public FoodStandDefaultSetting FoodStandDefaultSetting;
        public FoodStandCustomSetting FoodStandCustomSetting;
        
        private IBuildingStatsModule _buildingStatsModule;
        private IFoodStandInventoryModule _foodStandInventoryModule;
        private IItemController _itemController;
        private IInteractionTrade _interactionTrade;

        private FoodStandViewModel _foodStandViewModel;
        private FoodStandModel _foodStandModel;
        
        public void RegisterReference(IItemController itemController)
        {
            _itemController = itemController;
            
            InputItemKey = new List<Tuple<EMaterialType, EItemType>>();
            OutItemKey = new List<Tuple<EMaterialType, EItemType>>();
            
            InputItemKey.Add(new Tuple<EMaterialType, EItemType>(FoodStandCustomSetting.MaterialType, FoodStandCustomSetting.InputItemType));
            OutItemKey.Add(new Tuple<EMaterialType, EItemType>(FoodStandCustomSetting.MaterialType, FoodStandCustomSetting.OutputItemType));
            
            _buildingStatsModule = new BuildingStatsModule(FoodStandDefaultSetting.BuildingDataSo);
            _foodStandInventoryModule = new FoodStandInventoryModule(transform, transform, _buildingStatsModule, _itemController, InputItemKey, OutItemKey);

            _foodStandModel = new FoodStandModel();
            _foodStandViewModel = new FoodStandViewModel(_foodStandModel);
            FoodStandDefaultSetting.FoodStandView.BindViewModel(_foodStandViewModel);
            
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