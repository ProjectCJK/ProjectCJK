using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Modules;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.StatsModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Buildings;
using Units.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Buildings.UI.Stands;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IStand : IRegisterReference<IItemFactory>
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
        [SerializeField] private StandDefaultSetting _standDefaultSetting;
        [SerializeField] private StandCustomSetting _standCustomSetting;

        public EMaterialType MaterialType { get; private set; }
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutItemKey { get; protected set; }
        
        private IStandStatsModule _standStatsModule;
        private IStandInventoryModule _standInventoryModule;
        private IItemFactory _itemFactory;
        private IInteractionTrade _interactionTrade;

        private StandDataSO _standDataSo;
        private StandViewModel _standViewModel;
        private StandModel _standModel;
        
        public void RegisterReference(IItemFactory itemController)
        {
            _standDataSo = DataManager.Instance.StandData;
            
            _itemFactory = itemController;
            MaterialType = _standCustomSetting.MaterialType;
            BuildingKey = EnumParserModule.ParseDoubleEnumToString(_standDataSo.BuildingType, _standCustomSetting.MaterialType);
            InputItemKey = EnumParserModule.ParseDoubleEnumToString(_standCustomSetting.InputItemType, _standCustomSetting.MaterialType);
            OutItemKey =EnumParserModule.ParseDoubleEnumToString(_standCustomSetting.OutputItemType, _standCustomSetting.MaterialType);
            
            _standStatsModule = new StandStatsModule(_standDataSo);
            _standInventoryModule = new StandInventoryModule(_standDefaultSetting.standInventory, _standDefaultSetting.standInventory, _standStatsModule, _itemFactory, InputItemKey, OutItemKey);

            _standModel = new StandModel();
            _standViewModel = new StandViewModel(_standModel);
            _standDefaultSetting.standView.BindViewModel(_standViewModel);

            _interactionTrade = _standDefaultSetting.InteractionTrade;
            _interactionTrade.RegisterReference(_standInventoryModule.ReceiverTransform, _standInventoryModule, _standInventoryModule, BuildingKey, InputItemKey);

            _standInventoryModule.OnInventoryCountChanged += UpdateViewModel;
        }
        
        public override void Initialize() { }

        private void Update()
        {
            _standInventoryModule.Update();
        }

        private void UpdateViewModel()
        {
            var remainedProductCount = _standInventoryModule.GetItemCount(InputItemKey);
            _standViewModel.UpdateValues(remainedProductCount);
        }
    }
}