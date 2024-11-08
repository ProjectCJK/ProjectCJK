using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Controllers;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.UpgradeZones;
using UnityEngine;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Units
{
    public interface IWareHouse : IRegisterReference<ItemFactory>, IUnlockZoneProperty
    {
        
    }

    [Serializable]
    public struct WareHouseDefaultSetting
    {
        [Space(10), Header("아이템 보관 장소")]
        public Transform wareHouseInventory;
        
        [Space(10), Header("TradeZone_NPC")]
        public Transform TradeZone_NPC;
        
        [Space(10), Header("UnlockZone_Player")]
        public Transform UnlockZone_Player;
        
        [Space(10), Header("UpgradeZone_Player")]
        public Transform UpgradeZone_Player;
    }
    
    [Serializable]
    public struct WareHouseCustomSetting
    {
        
    }
    
    public class WareHouse : UnlockableBuildingZone, IWareHouse
    {
        [SerializeField] private WareHouseDefaultSetting _wareHouseDefaultSetting;
        [SerializeField] private WareHouseCustomSetting _wareHouseCustomSetting;
        
        public override UnlockZoneModule UnlockZoneModule { get; protected set; }
        public override EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public override EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public override int RequiredGoldForUnlock => UnlockZoneModule.RequiredGoldForUnlock;
        public override int CurrentGoldForUnlock { get; set; }
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Transform TradeZoneNpcTransform { get; }
        
        private WareHouseStatsModule _wareHouseStatsModule;
        private WareHouseInventoryModule _wareHouseInventoryModule;
        private ItemFactory _itemFactory;
        private TradeZone _tradeZone;
        private TradeZone _unlockZonePlayer;
        private UpgradeZone _upgradeZonePlayer;

        private WareHouseDataSO _wareHouseDataSo;
        // private WareHouseViewModel _wareHouseViewModel;
        // private WareHouseModel _wareHouseModel;
        
        private readonly HashSet<IHunter> _currentSpawnedHunters = new();
        
        public void RegisterReference(ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            _wareHouseDataSo = DataManager.Instance.WareHouseDataSo;
            _wareHouseStatsModule = new WareHouseStatsModule(_wareHouseDataSo, _wareHouseCustomSetting);
            
            BuildingKey = _wareHouseStatsModule.BuildingKey;

            UnlockZoneModule = GetComponent<UnlockZoneModule>();
            UnlockZoneModule.RegisterReference(BuildingKey);
            
            _wareHouseInventoryModule = new WareHouseInventoryModule(_wareHouseDefaultSetting.wareHouseInventory, _wareHouseDefaultSetting.wareHouseInventory, _itemFactory, _wareHouseStatsModule, InputItemKey, OutputItemKey);

            // _wareHouseModel = new WareHouseModel();
            // _wareHouseViewModel = new WareHouseViewModel(_wareHouseModel);
            // _wareHouseDefaultSetting.wareHouseView.BindViewModel(_wareHouseViewModel);

            _unlockZonePlayer = _wareHouseDefaultSetting.UnlockZone_Player.GetComponent<TradeZone>();
            _unlockZonePlayer.RegisterReference(this, _wareHouseDefaultSetting.UnlockZone_Player, _wareHouseInventoryModule, _wareHouseInventoryModule, BuildingKey, $"{ECurrencyType.Money}");
            
            _upgradeZonePlayer = _wareHouseDefaultSetting.UpgradeZone_Player.GetComponent<UpgradeZone>();
            
            // _wareHouseInventoryModule.OnInventoryCountChanged += UpdateViewModel;
            _wareHouseInventoryModule.OnMoneyReceived += HandleOnMoneyReceived;
            
            _upgradeZonePlayer.OnPlayerConnected += HandleOnPlayerConnected;
        }
        
        public override void Initialize() { }

        private void Update()
        {
            // _wareHouseInventoryModule.Update();
        }
        
        public void SpawnHunter(ICreatureController creatureController, HashSet<IHunter> currentSpawnedHunters)
        {
            if (_currentSpawnedHunters.Count < (int)_wareHouseStatsModule.CurrentWareHouseOption2Value)
            {
                IHunter deliveryMan = creatureController.GetHunter(transform.position);

                _currentSpawnedHunters.Add(deliveryMan);
                currentSpawnedHunters.Add(deliveryMan);
            }
        }
        
        private void HandleOnMoneyReceived(int value)
        {
            CurrentGoldForUnlock += value;
            UnlockZoneModule.CurrentGoldForUnlock = CurrentGoldForUnlock;
            
            UnlockZoneModule.UpdateViewModel();
            
            if (CurrentGoldForUnlock >= RequiredGoldForUnlock)
            {
                UnlockZoneModule.SetCurrentState(EActiveStatus.Active);
            }
        }
        
        private void HandleOnPlayerConnected(bool value)
        {
            if (value)
            {
                _wareHouseStatsModule.GetUIWareHouseEnhancement();
            }
            else
            {
                _wareHouseStatsModule.ReturnUIWareHouseEnhancement();
            }
        }
    }
}