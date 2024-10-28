using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.StatsModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Buildings;
using Units.Stages.Controllers;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Buildings.UI.Stands;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IStand : IRegisterReference<IItemController>
    {
        
    }
    
    [Serializable]
    public struct StandDefaultSetting
    {
        [Header("Stand UI")]
        public StandView standView;
        
        [Space(10), Header("아이템 보관 장소")]
        public Transform standInventory;
        
        [Space(10), Header("Interaction Zone")]
        public InteractionTrade InteractionTrade;
    }
    
    [Serializable]
    public struct StandCustomSetting
    {
        [Header("재료 타입")]
        public EMaterialType MaterialType;
        
        [Space(10), Header("Input 아이템 타입")]
        public EItemType InputItemType;
        
        [Space(10), Header("Output 아이템 타입")]
        public EItemType OutputItemType;
    }
    
    public class Stand : Building, IStand
    {
        [SerializeField] private StandDefaultSetting standDefaultSetting;
        [SerializeField] private StandCustomSetting standCustomSetting;
        
        public override EBuildingType BuildingType { get; protected set; }
        public override Tuple<EMaterialType, EItemType> InputItemKey { get; protected set; }
        public override Tuple<EMaterialType, EItemType> OutItemKey { get; protected set; }
        
        private IStandStatsModule _standStatsModule;
        private IStandInventoryModule _standInventoryModule;
        private IItemController _itemController;
        private IInteractionTrade _interactionTrade;

        private StandDataSO _standDataSo;
        private StandViewModel _standViewModel;
        private StandModel _standModel;
        
        public void RegisterReference(IItemController itemController)
        {
            _standDataSo = DataManager.Instance.StandData;
            
            BuildingType = _standDataSo.BuildingType;
            _itemController = itemController;
            
            InputItemKey = new Tuple<EMaterialType, EItemType>(standCustomSetting.MaterialType, standCustomSetting.InputItemType);
            OutItemKey = new Tuple<EMaterialType, EItemType>(standCustomSetting.MaterialType, standCustomSetting.OutputItemType);
            
            _standStatsModule = new StandStatsModule(_standDataSo);
            _standInventoryModule = new StandInventoryModule(standDefaultSetting.standInventory, standDefaultSetting.standInventory, _standStatsModule, _itemController, InputItemKey, OutItemKey);

            _standModel = new StandModel();
            _standViewModel = new StandViewModel(_standModel);
            standDefaultSetting.standView.BindViewModel(_standViewModel);

            _interactionTrade = standDefaultSetting.InteractionTrade;
            _interactionTrade.RegisterReference(_standInventoryModule.ReceiverTransform, _standInventoryModule, _standInventoryModule, InputItemKey);

            _standInventoryModule.OnInventoryCountChanged += UpdateViewModel;
        }
        
        public override void Initialize() { }
        
        private void UpdateViewModel()
        {
            var remainedProductCount = _standInventoryModule.GetItemCount(InputItemKey);
            _standViewModel.UpdateValues(remainedProductCount);
        }
    }
}