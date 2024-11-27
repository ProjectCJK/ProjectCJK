using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Controllers;
using Units.Stages.Enums;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.UpgradeZones;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IDeliveryLodging : IRegisterReference<ItemFactory>, IUnlockZoneProperty
    {
    }

    [Serializable]
    public struct DeliveryLodgingDefaultSetting
    {
        [Header("UnlockZone_Player")] public Transform UnlockZone_Player;

        [Space(10)] [Header("UpgradeZone_Player")]
        public Transform UpgradeZone_Player;

        public Animator Animator;
    }

    public class DeliveryLodging : UnlockableBuildingZone, IDeliveryLodging
    {
        [SerializeField] private DeliveryLodgingDefaultSetting _deliveryLodgingDefaultSetting;

        private readonly HashSet<IDeliveryMan> _currentSpawnedDeliveryMans = new();

        private DeliveryLodgingDataSO _deliveryLodgingDataSo;
        private DeliveryLodgingInventoryModule _deliveryLodgingInventoryModule;

        private DeliveryLodgingStatsModule _deliveryLodgingStatsModule;
        private ItemFactory _itemFactory;
        private TradeZone _unlockZonePlayer;
        private UpgradeZone _upgradeZonePlayer;

        public override UnlockZoneModule UnlockZoneModule { get; protected set; }
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Animator Animator => _deliveryLodgingDefaultSetting.Animator;
        public override Transform TradeZoneNpcTransform { get; }

        private void Update()
        {
            UnlockZoneModule.UpdateViewModel();
        }

        public override EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public override EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public override int RequiredGoldForUnlock => UnlockZoneModule.RequiredGoldForUnlock;
        public override int CurrentGoldForUnlock { get; set; }

        public void RegisterReference(ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            _deliveryLodgingDataSo = DataManager.Instance.DeliveryLodgingDataSo;
            _deliveryLodgingStatsModule = new DeliveryLodgingStatsModule(_deliveryLodgingDataSo);
            VolatileDataManager.Instance.DeliveryLodgingStatsModule = _deliveryLodgingStatsModule;
            BuildingKey = _deliveryLodgingStatsModule.BuildingKey;

            _deliveryLodgingInventoryModule = new DeliveryLodgingInventoryModule(null,
                _deliveryLodgingDefaultSetting.UnlockZone_Player, _itemFactory, _deliveryLodgingStatsModule, null,
                null);
            
            UnlockZoneModule = GetComponent<UnlockZoneModule>();
            _upgradeZonePlayer = _deliveryLodgingDefaultSetting.UpgradeZone_Player.GetComponent<UpgradeZone>();
            _unlockZonePlayer = _deliveryLodgingDefaultSetting.UnlockZone_Player.GetComponent<TradeZone>();

            UnlockZoneModule.RegisterReference(BuildingKey);
            _unlockZonePlayer.RegisterReference(this, _deliveryLodgingDefaultSetting.UnlockZone_Player,
                _deliveryLodgingInventoryModule, _deliveryLodgingInventoryModule, BuildingKey,
                $"{ECurrencyType.Money}");

            _deliveryLodgingInventoryModule.OnMoneyReceived += HandleOnMoneyReceived;

            _upgradeZonePlayer.OnPlayerConnected += HandleOnPlayerConnected;
        }

        public override void Initialize() { }

        public void SpawnDeliveryMan(ICreatureController creatureController, HashSet<IDeliveryMan> currentSpawnedDeliveryMans)
        {
            if (ActiveStatus == EActiveStatus.Active && _currentSpawnedDeliveryMans.Count < (int)_deliveryLodgingStatsModule.CurrentBuildingOption2Value)
            {
                IDeliveryMan deliveryMan = creatureController.GetDeliveryMan(transform.position);

                _currentSpawnedDeliveryMans.Add(deliveryMan);
                currentSpawnedDeliveryMans.Add(deliveryMan);
            }
        }

        private void HandleOnPlayerConnected(bool value)
        {
            if (value)
                _deliveryLodgingStatsModule.GetUIBuildingEnhancement();
            else
                _deliveryLodgingStatsModule.ReturnUIBuildingEnhancement();
        }
    }
}