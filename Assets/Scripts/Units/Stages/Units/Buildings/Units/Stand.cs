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
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.UI.Stands;
using Units.Stages.Units.Creatures.Enums;
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
        
        [Space(10), Header("TradeZone_Player")]
        public Transform TradeZone_Player;
        
        [Space(10), Header("TradeZone_NPC")]
        public Transform TradeZone_NPC;
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
        public override string OutputItemKey { get; protected set; }
        public override Transform TradeZoneZoneZoneZoneNpcTransform => _standDefaultSetting.TradeZone_NPC;

        private IStandStatsModule _standStatsModule;
        private IStandInventoryModule _standInventoryModule;
        private IItemFactory _itemFactory;
        private ITradeZone _tradeZonePlayer;
        private ITradeZone _tradeZoneNpc;

        private StandDataSO _standDataSo;
        private StandViewModel _standViewModel;
        private StandModel _standModel;
        
        public void RegisterReference(IItemFactory itemController)
        {
            _standDataSo = DataManager.Instance.StandData;
            
            _itemFactory = itemController;
            MaterialType = _standCustomSetting.MaterialType;
            BuildingKey = EnumParserModule.ParseEnumToString(_standDataSo.BuildingType, _standCustomSetting.MaterialType);
            InputItemKey = EnumParserModule.ParseEnumToString(_standCustomSetting.InputItemType, _standCustomSetting.MaterialType);
            OutputItemKey = EnumParserModule.ParseEnumToString(_standCustomSetting.OutputItemType, _standCustomSetting.MaterialType);
            
            _standStatsModule = new StandStatsModule(_standDataSo);
            _standInventoryModule = new StandInventoryModule(_standDefaultSetting.standInventory, _standDefaultSetting.standInventory, _standStatsModule, _itemFactory, InputItemKey, OutputItemKey);

            _standModel = new StandModel();
            _standViewModel = new StandViewModel(_standModel);
            _standDefaultSetting.standView.BindViewModel(_standViewModel);

            _tradeZonePlayer = _standDefaultSetting.TradeZone_Player.GetComponent<ITradeZone>();
            _tradeZonePlayer.RegisterReference(_standInventoryModule.ReceiverTransform, _standInventoryModule, _standInventoryModule, BuildingKey, InputItemKey);
            
            _tradeZoneNpc = _standDefaultSetting.TradeZone_NPC.GetComponent<ITradeZone>();
            _tradeZoneNpc.RegisterReference(_standInventoryModule.ReceiverTransform, _standInventoryModule, _standInventoryModule, BuildingKey, InputItemKey);
            
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