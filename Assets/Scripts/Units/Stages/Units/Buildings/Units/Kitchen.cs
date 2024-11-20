using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Enums;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.ProductModules;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.UpgradeZones;
using Units.Stages.Units.Buildings.UI.Kitchens;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IKitchen : IBuildingZone, IRegisterReference<ItemFactory>, IUnlockZoneProperty
    {
    }

    [Serializable]
    public struct KitchenDefaultSetting
    {
        [Header("Kitchen UI")]
        public KitchenView KitchenView;
        
        [Space(10), Header("Animator")]
        public Animator Animator;

        [Space(10), Header("아이템 생성 장소")]
        public Transform KitchenFactory;

        [Space(10), Header("아이템 보관 장소")]
        public Transform KitchenInventory;

        [Space(10), Header("Factory Animator")]
        public Animator FactoryAnimator;

        [Space(10), Header("TradeZone_Player")]
        public Transform TradeZone_Player;

        [Space(10), Header("TradeZone_NPC")] public Transform TradeZone_NPC;

        [Space(10), Header("UnlockZone_Player")]
        public Transform UnlockZone_Player;

        [Space(10)] [Header("UpgradeZone_Player")]
        public Transform UpgradeZone_Player;
    }

    [Serializable]
    public struct KitchenCustomSetting
    {
        [Header("재료 타입")] public EMaterialType MaterialType;

        [Space(10)] [Header("Input 아이템 타입")] public EItemType InputItemType;

        [Space(10)] [Header("Output 아이템 타입")] public EItemType OutputItemType;
        
        [Space(10)] [Header("인벤토리 아이템 진열")] public List<GameObject> SpawnedItem;
    }

    public class Kitchen : UnlockableBuildingZone, IKitchen
    {
        [SerializeField] private KitchenDefaultSetting _kitchenDefaultSetting;
        [SerializeField] private KitchenCustomSetting _kitchenCustomSetting;
        private ItemFactory _itemFactory;

        private KitchenDataSO _kitchenDataSO;
        private KitchenMaterialInventoryModule _kitchenMaterialInventoryModule;
        private KitchenModel _kitchenModel;
        private KitchenProductInventoryModule _kitchenProductInventoryModule;
        private KitchenProductModule _kitchenProductModule;

        private KitchenStatsModule _kitchenStatsModule;
        private KitchenViewModel _kitchenViewModel;
        private TradeZone _tradeZoneNpc;
        private TradeZone _tradeZonePlayer;
        private TradeZone _unlockZonePlayer;
        private UpgradeZone _upgradeZonePlayer;

        public override UnlockZoneModule UnlockZoneModule { get; protected set; }

        public EMaterialType MaterialType { get; private set; }

        public bool IsAnyItemOnInventory => _kitchenProductInventoryModule.CurrentInventorySize > 0;

        private void Update()
        {
#if UNITY_EDITOR
            // TODO : Test Scripts
            if (Input.GetKeyDown(KeyCode.W))
                _kitchenProductInventoryModule.ReceiveItemThroughTransfer(OutputItemKey, 1,
                    new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
#endif

            _kitchenProductInventoryModule.Update();
            _kitchenProductModule.Product();
        }

        public override EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public override EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public override int RequiredGoldForUnlock => UnlockZoneModule.RequiredGoldForUnlock;
        public override int CurrentGoldForUnlock { get; set; }
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Animator Animator => _kitchenDefaultSetting.Animator;
        public override Transform TradeZoneNpcTransform => _kitchenDefaultSetting.TradeZone_NPC;

        public void RegisterReference(ItemFactory itemController)
        {
            _itemFactory = itemController;
            _kitchenDataSO = DataManager.Instance.KitchenDataSo;
            _kitchenStatsModule = new KitchenStatsModule(_kitchenDataSO, _kitchenCustomSetting);
            VolatileDataManager.Instance.KitchenStatsModule.TryAdd(_kitchenStatsModule.MaterialType, _kitchenStatsModule);

            MaterialType = _kitchenStatsModule.MaterialType;
            BuildingKey = _kitchenStatsModule.BuildingKey;
            InputItemKey = _kitchenStatsModule.InputItemKey;
            OutputItemKey = _kitchenStatsModule.OutputItemKey;

            _kitchenMaterialInventoryModule = new KitchenMaterialInventoryModule(_kitchenDefaultSetting.KitchenFactory,
                _kitchenDefaultSetting.KitchenFactory, _kitchenStatsModule, _itemFactory, InputItemKey, OutputItemKey);
            _kitchenProductInventoryModule = new KitchenProductInventoryModule(_kitchenDefaultSetting.KitchenInventory,
                _kitchenDefaultSetting.KitchenInventory, _kitchenStatsModule, _itemFactory, OutputItemKey,
                OutputItemKey);
            _kitchenProductModule = new KitchenProductModule(_kitchenDefaultSetting.KitchenFactory,
                _kitchenDefaultSetting.KitchenFactory, _kitchenStatsModule, _kitchenMaterialInventoryModule,
                _kitchenProductInventoryModule, InputItemKey, OutputItemKey, _kitchenDefaultSetting.FactoryAnimator);

            UnlockZoneModule = GetComponent<UnlockZoneModule>();
            UnlockZoneModule.RegisterReference(BuildingKey);

            _kitchenModel = new KitchenModel();
            _kitchenViewModel = new KitchenViewModel(_kitchenModel);
            _kitchenDefaultSetting.KitchenView.BindViewModel(_kitchenViewModel);

            _tradeZonePlayer = _kitchenDefaultSetting.TradeZone_Player.GetComponent<TradeZone>();
            _tradeZonePlayer.RegisterReference(this, _kitchenDefaultSetting.KitchenFactory,
                _kitchenMaterialInventoryModule, _kitchenProductInventoryModule, BuildingKey, InputItemKey);

            _tradeZoneNpc = _kitchenDefaultSetting.TradeZone_NPC.GetComponent<TradeZone>();
            _tradeZoneNpc.RegisterReference(this, _kitchenDefaultSetting.KitchenFactory,
                _kitchenMaterialInventoryModule, _kitchenProductInventoryModule, BuildingKey, InputItemKey);

            _unlockZonePlayer = _kitchenDefaultSetting.UnlockZone_Player.GetComponent<TradeZone>();
            _unlockZonePlayer.RegisterReference(this, _kitchenDefaultSetting.UnlockZone_Player,
                _kitchenMaterialInventoryModule, _kitchenProductInventoryModule, BuildingKey, $"{ECurrencyType.Money}");

            _upgradeZonePlayer = _kitchenDefaultSetting.UpgradeZone_Player.GetComponent<UpgradeZone>();

            _kitchenProductModule.OnProcessingChanged += OnProcessingStateChanged;
            _kitchenProductModule.OnElapsedTimeChanged += UpdateViewModel;

            _kitchenMaterialInventoryModule.OnMoneyReceived += HandleOnMoneyReceived;

            _upgradeZonePlayer.OnPlayerConnected += HandleOnPlayerConnected;

            _kitchenStatsModule.OnTriggerBuildingAnimation += HandleOnTriggerBuildingAnimation;

            _kitchenProductInventoryModule.OnUpdateStackedItem += HandleOnUpdateStackedItem;
        }

        public override void Initialize()
        {
            HandleOnUpdateStackedItem(_kitchenProductInventoryModule.CurrentInventorySize);
            UpdateViewModel();
            UnlockZoneModule.UpdateViewModel();
        }

        private void UpdateViewModel()
        {
            var remainedMaterialCount = _kitchenMaterialInventoryModule.GetItemCount(InputItemKey);
            var elapsedTime = _kitchenProductModule.ElapsedTime;
            var productLeadTime = _kitchenProductModule.ProductLeadTime;
            _kitchenViewModel.UpdateValues(remainedMaterialCount, elapsedTime, productLeadTime);
        }

        private void OnProcessingStateChanged(bool isProcessing)
        {
            _kitchenDefaultSetting.KitchenView.gameObject.SetActive(isProcessing);
        }

        private void HandleOnPlayerConnected(bool value)
        {
            if (value)
                _kitchenStatsModule.GetUIBuildingEnhancement();
            else
                _kitchenStatsModule.ReturnUIBuildingEnhancement();
        }

        private void HandleOnUpdateStackedItem(int value)
        {
            var targetIndex = Mathf.Min(_kitchenCustomSetting.SpawnedItem.Count, value) - 1;

            foreach (GameObject spawnedItem in _kitchenCustomSetting.SpawnedItem)
            {
                spawnedItem.SetActive(false);
            }
            
            if (targetIndex >= 0)_kitchenCustomSetting.SpawnedItem[targetIndex].SetActive(true);
        }
    }
}