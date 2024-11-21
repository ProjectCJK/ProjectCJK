using System;
using System.Collections.Generic;
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
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.UI.Stands;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IStand : IRegisterReference<ItemFactory>, IUnlockZoneProperty
    {
    }

    [Serializable]
    public struct StandDefaultSetting
    {
        [Header("Stand UI")]
        public StandView standView;
        
        [Space(10), Header("Animator")]
        public Animator Animator;

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
        [Header("재료 타입")] public EMaterialType MaterialType;

        [Space(10)] [Header("Input 아이템 타입")] public EItemType InputItemType;

        [Space(10)] [Header("Output 아이템 타입")] public EItemType OutputItemType;
        
        [Space(10)] [Header("인벤토리 아이템 진열")] public List<GameObject> SpawnedItem;
    }

    public class Stand : UnlockableBuildingZone, IStand
    {
        [SerializeField] private StandDefaultSetting _standDefaultSetting;
        [SerializeField] private StandCustomSetting _standCustomSetting;
        private ItemFactory _itemFactory;

        private StandDataSO _standDataSo;
        private StandInventoryModule _standInventoryModule;
        private StandModel _standModel;

        private StandStatsModule _standStatsModule;
        private StandViewModel _standViewModel;
        private TradeZone _tradeZoneNpc;
        private TradeZone _tradeZonePlayer;
        private TradeZone _unlockZonePlayer;

        public EMaterialType MaterialType { get; private set; }

        public override UnlockZoneModule UnlockZoneModule { get; protected set; }
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Animator Animator => _standDefaultSetting.Animator;
        public override Transform TradeZoneNpcTransform => _standDefaultSetting.TradeZone_NPC;

        private void Update()
        {
            _standInventoryModule.Update();
            UnlockZoneModule.UpdateViewModel();
        }

        public override EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public override EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public override int RequiredGoldForUnlock => UnlockZoneModule.RequiredGoldForUnlock;
        public override int CurrentGoldForUnlock { get; set; }

        public void RegisterReference(ItemFactory itemFactory)
        {
            _standDataSo = DataManager.Instance.StandDataSo;

            _itemFactory = itemFactory;
            MaterialType = _standCustomSetting.MaterialType;
            BuildingKey = ParserModule.ParseEnumToString(_standDataSo.BuildingType, _standCustomSetting.MaterialType);
            InputItemKey =
                ParserModule.ParseEnumToString(_standCustomSetting.InputItemType, _standCustomSetting.MaterialType);
            OutputItemKey =
                ParserModule.ParseEnumToString(_standCustomSetting.OutputItemType, _standCustomSetting.MaterialType);

            UnlockZoneModule = GetComponent<UnlockZoneModule>();
            UnlockZoneModule.RegisterReference(BuildingKey);

            _standStatsModule = new StandStatsModule(_standDataSo, _standCustomSetting);
            _standInventoryModule = new StandInventoryModule(_standDefaultSetting.standInventory,
                _standDefaultSetting.standInventory, _standStatsModule, _itemFactory, InputItemKey, OutputItemKey);

            _standModel = new StandModel();
            _standViewModel = new StandViewModel(_standModel);
            _standDefaultSetting.standView.BindViewModel(_standViewModel);

            _tradeZonePlayer = _standDefaultSetting.TradeZone_Player.GetComponent<TradeZone>();
            _tradeZonePlayer.RegisterReference(this, _standInventoryModule.ReceiverTransform, _standInventoryModule,
                _standInventoryModule, BuildingKey, InputItemKey);

            _tradeZoneNpc = _standDefaultSetting.TradeZone_NPC.GetComponent<TradeZone>();
            _tradeZoneNpc.RegisterReference(this, _standInventoryModule.ReceiverTransform, _standInventoryModule,
                _standInventoryModule, BuildingKey, InputItemKey);

            _unlockZonePlayer = _standDefaultSetting.UnlockZone_Player.GetComponent<TradeZone>();
            _unlockZonePlayer.RegisterReference(this, _standDefaultSetting.UnlockZone_Player, _standInventoryModule,
                _standInventoryModule, BuildingKey, $"{ECurrencyType.Money}");

            _standInventoryModule.OnInventoryCountChanged += UpdateViewModel;
            _standInventoryModule.OnMoneyReceived += HandleOnMoneyReceived;

            _standInventoryModule.OnUpdateStackedItem += HandleOnUpdateStackedItem;
        }

        public override void Initialize()
        {
            HandleOnUpdateStackedItem(_standInventoryModule.CurrentInventorySize);
            UpdateViewModel();
        }

        private void UpdateViewModel()
        {
            var remainedProductCount = _standInventoryModule.GetItemCount(InputItemKey);
            _standViewModel.UpdateValues(remainedProductCount);
        }
        
        private void HandleOnUpdateStackedItem(int value)
        {
            var targetIndex = Mathf.Min(_standCustomSetting.SpawnedItem.Count, value) - 1;

            foreach (GameObject spawnedItem in _standCustomSetting.SpawnedItem)
            {
                spawnedItem.SetActive(false);
            }
            
            if (targetIndex >= 0)_standCustomSetting.SpawnedItem[targetIndex].SetActive(true);
        }
    }
}