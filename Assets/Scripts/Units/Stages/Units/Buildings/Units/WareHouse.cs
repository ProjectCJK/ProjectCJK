using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Controllers;
using Units.Stages.Enums;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Units;
using Units.Stages.Units.Buildings.Modules.UpgradeZones;
using Units.Stages.Units.Buildings.UI.WareHouses;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IWareHouse : IRegisterReference<ItemFactory>, IUnlockZoneProperty
    {
    }

    [Serializable]
    public struct WareHouseDefaultSetting
    {
        public GameObject uiCanvasWareHouseView;

        [Space(10)] [Header("아이템 보관 장소")] public Transform wareHouseInventory;

        [Space(10)] [Header("TradeZone_NPC")] public Transform TradeZone_NPC;

        [Space(10)] [Header("UnlockZone_Player")]
        public Transform UnlockZone_Player;

        [Space(10)] [Header("UpgradeZone_Player")]
        public Transform UpgradeZone_Player;


        [Space(10)] [Header("Interaction_Module")]
        public InteractionModule InteractionModule;
    }

    [Serializable]
    public struct WareHouseCustomSetting
    {
    }

    public class WareHouse : UnlockableBuildingZone, IWareHouse
    {
        [SerializeField] private WareHouseDefaultSetting _wareHouseDefaultSetting;
        [SerializeField] private WareHouseCustomSetting _wareHouseCustomSetting;

        private readonly HashSet<IHunter> _currentSpawnedHunters = new();
        private InteractionModule _interactionModule;
        private ItemFactory _itemFactory;
        private TradeZone _tradeZoneNpc;
        private TradeZone _tradeZonePlayer;
        private TradeZone _unlockZonePlayer;
        private UpgradeZone _upgradeZonePlayer;

        private WareHouseDataSO _wareHouseDataSo;
        private WareHouseInventoryModule _wareHouseInventoryModule;

        private WareHouseStatsModule _wareHouseStatsModule;
        private WareHouseView _wareHouseView;
        private WareHouseViewModel _wareHouseViewModel;

        private bool isPlayerInCollision;
        private bool isSendingItemToPlayer;
        private ICreatureItemReceiver player;
        private EMaterialType sendingItemType;

        public override UnlockZoneModule UnlockZoneModule { get; protected set; }
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Transform TradeZoneNpcTransform { get; }

        private void Update()
        {
            if (isSendingItemToPlayer) _wareHouseInventoryModule.SendItem(player, sendingItemType);
            if (!_wareHouseInventoryModule.HasMatchingItem($"{EItemType.Material}_{sendingItemType}") ||
                !player.CanReceiveItem()) isSendingItemToPlayer = false;

            if (isPlayerInCollision) _wareHouseViewModel.UpdateButtonData();
        }

        public override EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public override EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public override int RequiredGoldForUnlock => UnlockZoneModule.RequiredGoldForUnlock;
        public override int CurrentGoldForUnlock { get; set; }

        public void RegisterReference(ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            _wareHouseDataSo = DataManager.Instance.WareHouseDataSo;
            _wareHouseStatsModule = new WareHouseStatsModule(_wareHouseDataSo, _wareHouseCustomSetting);

            BuildingKey = _wareHouseStatsModule.BuildingKey;

            UnlockZoneModule = GetComponent<UnlockZoneModule>();
            UnlockZoneModule.RegisterReference(BuildingKey);

            _wareHouseInventoryModule = new WareHouseInventoryModule(_wareHouseDefaultSetting.wareHouseInventory,
                _wareHouseDefaultSetting.wareHouseInventory, _itemFactory, _wareHouseStatsModule, InputItemKey,
                OutputItemKey);

            _wareHouseViewModel = new WareHouseViewModel(_wareHouseInventoryModule, HandleOnClickWareHouseViewButtons);
            _wareHouseView = _wareHouseDefaultSetting.uiCanvasWareHouseView.GetComponent<WareHouseView>();
            _wareHouseView.BindViewModel(_wareHouseViewModel);

            _tradeZoneNpc = _wareHouseDefaultSetting.TradeZone_NPC.GetComponent<TradeZone>();
            _tradeZoneNpc.RegisterReference(this, _wareHouseInventoryModule.ReceiverTransform,
                _wareHouseInventoryModule, _wareHouseInventoryModule, BuildingKey, InputItemKey);

            _unlockZonePlayer = _wareHouseDefaultSetting.UnlockZone_Player.GetComponent<TradeZone>();
            _unlockZonePlayer.RegisterReference(this, _wareHouseDefaultSetting.UnlockZone_Player,
                _wareHouseInventoryModule, _wareHouseInventoryModule, BuildingKey, $"{ECurrencyType.Money}");

            _upgradeZonePlayer = _wareHouseDefaultSetting.UpgradeZone_Player.GetComponent<UpgradeZone>();

            _wareHouseInventoryModule.OnMoneyReceived += HandleOnMoneyReceived;
            _upgradeZonePlayer.OnPlayerConnected += HandleOnPlayerConnected;

            _interactionModule = _wareHouseDefaultSetting.InteractionModule;
            _interactionModule.OnTriggerStay2DAction += HandleOnTriggerStay2D;
            _interactionModule.OnTriggerExit2DAction += HandleOnTriggerExit2D;
        }

        public override void Initialize()
        {
        }

        public void SpawnHunter(ICreatureController creatureController, HashSet<IHunter> currentSpawnedHunters)
        {
            if (_currentSpawnedHunters.Count < (int)_wareHouseStatsModule.CurrentWareHouseOption2Value)
            {
                IHunter hunter = creatureController.GetHunter(transform.position);

                _currentSpawnedHunters.Add(hunter);
                currentSpawnedHunters.Add(hunter);
            }
        }

        private void HandleOnMoneyReceived(int value)
        {
            CurrentGoldForUnlock += value;
            UnlockZoneModule.CurrentGoldForUnlock = CurrentGoldForUnlock;

            UnlockZoneModule.UpdateViewModel();

            if (CurrentGoldForUnlock >= RequiredGoldForUnlock) UnlockZoneModule.SetCurrentState(EActiveStatus.Active);
        }

        private void HandleOnPlayerConnected(bool value)
        {
            if (value)
                _wareHouseStatsModule.GetUIWareHouseEnhancement();
            else
                _wareHouseStatsModule.ReturnUIWareHouseEnhancement();
        }

        private void HandleOnTriggerStay2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Player")) return;

            if (!isPlayerInCollision)
            {
                isPlayerInCollision = true;
                player = col.GetComponent<Player>().CreatureItemReceiver;
                if (!_wareHouseDefaultSetting.uiCanvasWareHouseView.gameObject.activeInHierarchy)
                    _wareHouseDefaultSetting.uiCanvasWareHouseView.SetActive(true);
            }
        }

        private void HandleOnTriggerExit2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Player")) return;

            isPlayerInCollision = false;
            _wareHouseDefaultSetting.uiCanvasWareHouseView.SetActive(false);
        }

        private void HandleOnClickWareHouseViewButtons(EMaterialType materialType)
        {
            isSendingItemToPlayer = true;
            sendingItemType = materialType;
        }
    }
}