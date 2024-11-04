using System;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.UI.Stands;
using UnityEngine;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Units
{
    public interface IStand : IRegisterReference<IItemFactory>, IUnlockZoneProperty
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
        
        [Space(10), Header("UnlockZone_Player")]
        public Transform UnlockZone_Player;
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
    
    public class Stand : UnlockableBuildingZoneProperty, IStand
    {
        [SerializeField] private StandDefaultSetting _standDefaultSetting;
        [SerializeField] private StandCustomSetting _standCustomSetting;

        public override UnlockZoneModule UnlockZoneModule { get; protected set; }
        public override EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public override EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public override int RequiredGoldForUnlock => UnlockZoneModule.RequiredGoldForUnlock;

        public override int CurrentGoldForUnlock { get; set; }
        public EMaterialType MaterialType { get; private set; }
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Transform TradeZoneNpcTransform => _standDefaultSetting.TradeZone_NPC;

        private IStandStatsModule _standStatsModule;
        private IStandInventoryModule _standInventoryModule;
        private IItemFactory _itemFactory;
        private ITradeZone _tradeZonePlayer;
        private ITradeZone _tradeZoneNpc;

        private StandDataSO _standDataSo;
        private StandViewModel _standViewModel;
        private StandModel _standModel;
        
        public void RegisterReference(IItemFactory itemFactory)
        {
            _standDataSo = DataManager.Instance.StandDataSo;
            
            _itemFactory = itemFactory;
            MaterialType = _standCustomSetting.MaterialType;
            BuildingKey = EnumParserModule.ParseEnumToString(_standDataSo.BuildingType, _standCustomSetting.MaterialType);
            InputItemKey = EnumParserModule.ParseEnumToString(_standCustomSetting.InputItemType, _standCustomSetting.MaterialType);
            OutputItemKey = EnumParserModule.ParseEnumToString(_standCustomSetting.OutputItemType, _standCustomSetting.MaterialType);

            UnlockZoneModule = GetComponent<UnlockZoneModule>();
            UnlockZoneModule.RegisterReference();
            
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