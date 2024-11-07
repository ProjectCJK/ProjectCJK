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
using Units.Stages.Units.Zones.Units.BuildingZones.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.UI.Stands;
using UnityEngine;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Units
{
    public interface IDeliveryLodging : IRegisterReference<ItemFactory>, IUnlockZoneProperty
    {
        
    }
    
    [Serializable]
    public struct DeliveryLodgingDefaultSetting
    {
        [Header("UnlockZone_Player")]
        public Transform UnlockZone_Player;
    }
    
    public class DeliveryLodging : UnlockableBuildingZone, IDeliveryLodging
    {
        [SerializeField] private DeliveryLodgingDefaultSetting _deliveryLodgingDefaultSetting;
        
        public override UnlockZoneModule UnlockZoneModule { get; protected set; }
        public override EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public override EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public override int RequiredGoldForUnlock => UnlockZoneModule.RequiredGoldForUnlock;
        public override int CurrentGoldForUnlock { get; set; }
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Transform TradeZoneNpcTransform { get; }

        public int MaxDeliveryManCount => _deliveryLodgingStatsModule.BaseMaxDeliveryManCount;

        private DeliveryLodgingStatsModule _deliveryLodgingStatsModule;
        private DeliveryLodgingInventoryModule _deliveryLodgingInventoryModule;
        private ItemFactory _itemFactory;
        private TradeZone _unlockZonePlayer;
        
        private DeliveryLodgingDataSO _deliveryLodgingDataSo;

        public void RegisterReference(ItemFactory itemFactory)
        {
            _deliveryLodgingDataSo = DataManager.Instance.DeliveryLodgingDataSo;
            
            _itemFactory = itemFactory;
            BuildingKey = $"{EBuildingType.DeliveryLodging}";
            
            UnlockZoneModule = GetComponent<UnlockZoneModule>();
            UnlockZoneModule.RegisterReference(BuildingKey);

            _deliveryLodgingStatsModule = new DeliveryLodgingStatsModule(_deliveryLodgingDataSo);
            _deliveryLodgingInventoryModule = new DeliveryLodgingInventoryModule(null, _deliveryLodgingDefaultSetting.UnlockZone_Player, _itemFactory, _deliveryLodgingStatsModule, null, null);
            
            _unlockZonePlayer = _deliveryLodgingDefaultSetting.UnlockZone_Player.GetComponent<TradeZone>();
            _unlockZonePlayer.RegisterReference(this, _deliveryLodgingDefaultSetting.UnlockZone_Player, _deliveryLodgingInventoryModule, _deliveryLodgingInventoryModule, BuildingKey, $"{ECurrencyType.Money}");
            
            _deliveryLodgingInventoryModule.OnMoneyReceived += HandleOnMoneyReceived;
        }

        public override void Initialize() { }

        private void Update()
        {
            UnlockZoneModule.UpdateViewModel();
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
    }
}