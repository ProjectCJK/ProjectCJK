using System;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.PaymentModule.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.UpgradeZones;
using UnityEngine;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Units
{
    public interface IManagementDesk : IBuildingZone, IRegisterReference<ItemFactory>
    {
        
    }

    [Serializable]
    public struct ManagementDeskDefaultSetting
    {
        [Space(10), Header("아이템 보관 장소")]
        public Transform managementDeskInventory;
        
        [Space(10), Header("TradeZone_Player")]
        public Transform TradeZone_Player;
        
        [Space(10), Header("PaymentZone_Player")]
        public Transform PaymentZone_Player;
        
        [Space(10), Header("PaymentZone_NPC")]
        public Transform PaymentZone_NPC;

        [Space(10), Header("UpgradeZone_Player")]
        public Transform UpgradeZone_Player;
    }
    
    [Serializable]
    public struct ManagementDeskCustomSetting
    {
        [Header("재화 타입")]
        public ECurrencyType CurrencyType;
    }
    
    public class ManagementDesk : BuildingZone, IManagementDesk
    {
        [SerializeField] private ManagementDeskDefaultSetting _managementDeskDefaultSetting;
        [SerializeField] private ManagementDeskCustomSetting _managementDeskCustomSetting;
        
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Transform TradeZoneNpcTransform => _managementDeskDefaultSetting.PaymentZone_NPC;

        private ManagementDeskStatsModule _managementDeskStatsModule;
        private ManagementDeskPaymentModule _managementDeskPaymentModule;
        private ManagementDeskInventoryModule _managementDeskInventoryModule;
        private ItemFactory _itemFactory;
        private TradeZone _tradeZonePlayer;
        private PaymentZone _paymentZonePlayer;
        private PaymentZone _paymentZoneNpc;
        private UpgradeZone _upgradeZonePlayer;
        
        private ManagementDeskDataSO _managementDeskDataSo;
        
        // private HashSet<ICreatureItemReceiver>

        public void RegisterReference(ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            _managementDeskDataSo = DataManager.Instance.ManagementDeskDataSo;
            _managementDeskStatsModule = new ManagementDeskStatsModule(_managementDeskDataSo, _managementDeskCustomSetting);

            BuildingKey = _managementDeskStatsModule.BuildingKey;
            InputItemKey = _managementDeskStatsModule.InputItemKey;
            OutputItemKey = _managementDeskStatsModule.OutputItemKey;
            
            _managementDeskInventoryModule = new ManagementDeskInventoryModule(_managementDeskDefaultSetting.managementDeskInventory, _managementDeskDefaultSetting.managementDeskInventory, _managementDeskStatsModule, _itemFactory, InputItemKey, OutputItemKey);
            _managementDeskPaymentModule = new ManagementDeskPaymentModule(_managementDeskStatsModule, _managementDeskInventoryModule, InputItemKey);
            
            _tradeZonePlayer = _managementDeskDefaultSetting.TradeZone_Player.GetComponent<TradeZone>();
            _tradeZonePlayer.RegisterReference(null, _managementDeskInventoryModule.ReceiverTransform, _managementDeskInventoryModule, _managementDeskInventoryModule, BuildingKey, InputItemKey);
            
            _upgradeZonePlayer = _managementDeskDefaultSetting.UpgradeZone_Player.GetComponent<UpgradeZone>();
            
            _paymentZonePlayer = _managementDeskDefaultSetting.PaymentZone_Player.GetComponent<PaymentZone>();
            _paymentZonePlayer.RegisterReference(_managementDeskPaymentModule, BuildingKey);
            _paymentZoneNpc = _managementDeskDefaultSetting.PaymentZone_NPC.GetComponent<PaymentZone>();
            _paymentZoneNpc.RegisterReference(_managementDeskPaymentModule, BuildingKey);
            
            _upgradeZonePlayer.OnPlayerConnected += HandleOnPlayerConnected;
        }

        public override void Initialize() { }
        
        private void Update()
        {
            _managementDeskPaymentModule.Update();
            _managementDeskInventoryModule.Update();
            
#if UNITY_EDITOR
            // TODO : Test Scripts
            if (Input.GetKeyDown(KeyCode.W))
            {
                _managementDeskInventoryModule.ReceiveItemNoThroughTransfer(OutputItemKey, VolatileDataManager.Instance.GetItemPrice(EItemType.Product, EMaterialType.A));
            }
#endif
        }
        
        private void HandleOnPlayerConnected(bool value)
        {
            if (value)
            {
                _managementDeskStatsModule.GetUIManagementDeskEnhancement();
            }
            else
            {
                _managementDeskStatsModule.ReturnUIManagementDeskEnhancement();
            }
        }
    }
}