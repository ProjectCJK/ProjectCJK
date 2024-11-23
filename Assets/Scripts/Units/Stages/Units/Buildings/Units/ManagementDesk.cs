using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.PaymentModule.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.UpgradeZones;
using Units.Stages.Units.Buildings.UI.ManagementDesks;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IManagementDesk : IBuildingZone, IRegisterReference<ItemFactory>
    {
    }

    [Serializable]
    public struct ManagementDeskDefaultSetting
    {
        [Space(10)] [Header("아이템 보관 장소")] public Transform managementDeskInventory;

        [Space(10)] [Header("TradeZone_Player")]
        public Transform TradeZone_Player;

        [Space(10)] [Header("PaymentZone_Player")]
        public Transform PaymentZone_Player;

        [Space(10)] [Header("PaymentZone_NPC")]
        public Transform PaymentZone_NPC;

        [Space(10)] [Header("UpgradeZone_Player")]
        public Transform UpgradeZone_Player;

        public Animator Animator;
    }

    [Serializable]
    public struct ManagementDeskCustomSetting
    {
        [Header("재화 타입")] public ECurrencyType CurrencyType;

        [Header("계산원")] public List<Cashier> Cashier;
        
        [Space(10)] [Header("인벤토리 아이템 진열")] public List<GameObject> SpawnedItem;
        [Space(10)] [Header("인벤토리 아이템 활성화 조건")] public List<int> triggetCount;
    }

    public class ManagementDesk : BuildingZone, IManagementDesk
    {
        [SerializeField] private ManagementDeskDefaultSetting _managementDeskDefaultSetting;
        [SerializeField] private ManagementDeskCustomSetting _managementDeskCustomSetting;
        [SerializeField] private List<PaymentView> _paymentView;
        private ItemFactory _itemFactory;

        private ManagementDeskDataSO _managementDeskDataSo;
        private ManagementDeskInventoryModule _managementDeskInventoryModule;
        private ManagementDeskPaymentModule _managementDeskPaymentModule;

        private ManagementDeskStatsModule _managementDeskStatsModule;
        private PaymentZone _paymentZoneNpc;
        private PaymentZone _paymentZonePlayer;
        private TradeZone _tradeZonePlayer;
        private UpgradeZone _upgradeZonePlayer;

        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Animator Animator => _managementDeskDefaultSetting.Animator;
        public override Transform TradeZoneNpcTransform => _managementDeskDefaultSetting.PaymentZone_NPC;

        public void RegisterReference(ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            _managementDeskDataSo = DataManager.Instance.ManagementDeskDataSo;
            _managementDeskStatsModule = new ManagementDeskStatsModule(_managementDeskDataSo, _managementDeskCustomSetting);
            VolatileDataManager.Instance.ManagementDeskStatsModule = _managementDeskStatsModule;
            
            BuildingKey = _managementDeskStatsModule.BuildingKey;
            InputItemKey = _managementDeskStatsModule.InputItemKey;
            OutputItemKey = _managementDeskStatsModule.OutputItemKey;

            _managementDeskInventoryModule = new ManagementDeskInventoryModule(
                _managementDeskDefaultSetting.managementDeskInventory,
                _managementDeskDefaultSetting.managementDeskInventory, _managementDeskStatsModule, _itemFactory,
                InputItemKey, OutputItemKey);
            _managementDeskPaymentModule = new ManagementDeskPaymentModule(_managementDeskStatsModule,
                _managementDeskInventoryModule, InputItemKey, _paymentView);

            _tradeZonePlayer = _managementDeskDefaultSetting.TradeZone_Player.GetComponent<TradeZone>();
            _upgradeZonePlayer = _managementDeskDefaultSetting.UpgradeZone_Player.GetComponent<UpgradeZone>();
            _paymentZonePlayer = _managementDeskDefaultSetting.PaymentZone_Player.GetComponent<PaymentZone>();
            _paymentZoneNpc = _managementDeskDefaultSetting.PaymentZone_NPC.GetComponent<PaymentZone>();

            _paymentZonePlayer.RegisterReference(_managementDeskPaymentModule, BuildingKey);
            _paymentZoneNpc.RegisterReference(_managementDeskPaymentModule, BuildingKey);
            _tradeZonePlayer.RegisterReference(null, _managementDeskInventoryModule.ReceiverTransform,
                _managementDeskInventoryModule, _managementDeskInventoryModule, BuildingKey, InputItemKey);

            _upgradeZonePlayer.OnPlayerConnected += HandleOnPlayerConnected;

            _managementDeskInventoryModule.OnUpdateStackedItem += HandleOnUpdateStackedItem;
        }

        public override void Initialize()
        {
            HandleOnUpdateStackedItem(_managementDeskInventoryModule.CurrentInventorySize);
        }
        
        private void Update()
        {
            _managementDeskPaymentModule.Update();
            _managementDeskInventoryModule.Update();

            SpawnCashier();

#if UNITY_EDITOR
            // TODO : Test Scripts
            if (Input.GetKeyDown(KeyCode.W))
                _managementDeskInventoryModule.ReceiveItemNoThroughTransfer(OutputItemKey,
                    VolatileDataManager.Instance.GetItemPrice(EItemType.ProductA, EMaterialType.A));
#endif
        }

        private void SpawnCashier()
        {
            if (_managementDeskStatsModule.CurrentBuildingOption2Value > _managementDeskPaymentModule.CurrentSpawnedCashierCount)
            {
                for (var i = 0; i < _managementDeskStatsModule.CurrentBuildingOption2Value; i++)
                {
                    if (!_managementDeskCustomSetting.Cashier[i].gameObject.activeSelf)
                    {
                        _managementDeskCustomSetting.Cashier[i].gameObject.SetActive(true);
                        _managementDeskPaymentModule.AddCashierPaymentSlot(i + 1, _managementDeskCustomSetting.Cashier[i].Animator);
                    }
                    else break;
                }
            }
        }

        private void HandleOnPlayerConnected(bool value)
        {
            if (value)
                _managementDeskStatsModule.GetUIBuildingEnhancement();
            else
                _managementDeskStatsModule.ReturnUIBuildingEnhancement();
        }
        
        private void HandleOnUpdateStackedItem(int value)
        {
            foreach (GameObject spawnedItem in _managementDeskCustomSetting.SpawnedItem)
            {
                spawnedItem.SetActive(false);
            }

            if (value > 0)
            {
                var targetIndex = 0;
                
                for (var index = 0; index < _managementDeskCustomSetting.triggetCount.Count; index++)
                {
                    if (_managementDeskCustomSetting.triggetCount[index] <= value)
                    {
                        targetIndex = index;
                    }
                    else if (_managementDeskCustomSetting.triggetCount[index] > value) break;
                }

                _managementDeskCustomSetting.SpawnedItem[targetIndex].SetActive(true);
            }
        }
    }
}