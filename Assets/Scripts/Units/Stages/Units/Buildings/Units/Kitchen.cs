using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.ProductModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Buildings;
using Units.Stages.Controllers;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Buildings.UI.Kitchens;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IKitchen : IBuilding, IRegisterReference<IItemFactory>
    {
        
    }

    [Serializable]
    public struct KitchenDefaultSetting
    {
        [Header("Kitchen UI")]
        public KitchenView kitchenView;
        
        [Space(10), Header("아이템 생성 장소")]
        public Transform kitchenFactory;
        
        [Space(10), Header("아이템 보관 장소")]
        public Transform kitchenInventory;
        
        [Space(10), Header("Interaction Zone")]
        public InteractionTrade InteractionTrade;
    }

    [Serializable]
    public struct KitchenCustomSetting
    {
        [Header("재료 타입")]
        public EMaterialType MaterialType;
        
        [Space(10), Header("Input 아이템 타입")]
        public EItemType InputItemType;
        
        [Space(10), Header("Output 아이템 타입")]
        public EItemType OutputItemType;
    }
    
    public class Kitchen : Building, IKitchen
    {
        [SerializeField] private KitchenDefaultSetting _kitchenDefaultSetting;
        [SerializeField] private KitchenCustomSetting _kitchenCustomSetting;

        public override EBuildingType BuildingType => _kitchenDataSO.BuildingType;
        public override EMaterialType BuildingMaterialType => _kitchenCustomSetting.MaterialType;
        public override Tuple<EMaterialType, EItemType> InputItemKey { get; protected set; }
        public override Tuple<EMaterialType, EItemType> OutItemKey { get; protected set; }
        
        private IKitchenStatsModule _kitchenStatsModule;
        private IKitchenMaterialInventoryModule _kitchenMaterialInventoryModule;
        private IKitchenProductInventoryModule _kitchenProductInventoryModule;
        private IKitchenProductModule _kitchenProductModule;
        private IItemFactory _itemFactory;
        private IInteractionTrade _interactionTrade;
        
        private KitchenDataSO _kitchenDataSO;
        private KitchenViewModel _kitchenViewModel;
        private KitchenModel _kitchenModel;

        public void RegisterReference(IItemFactory itemController)
        {
            _kitchenDataSO = DataManager.Instance.KitchenData;
            
            _itemFactory = itemController;

            InputItemKey = new Tuple<EMaterialType, EItemType>(_kitchenCustomSetting.MaterialType, _kitchenCustomSetting.InputItemType);
            OutItemKey = new Tuple<EMaterialType, EItemType>(_kitchenCustomSetting.MaterialType, _kitchenCustomSetting.OutputItemType);
            
            _kitchenStatsModule = new KitchenStatsModule(_kitchenDataSO);
            _kitchenMaterialInventoryModule = new KitchenMaterialInventoryModule(_kitchenDefaultSetting.kitchenFactory, _kitchenDefaultSetting.kitchenFactory, _kitchenStatsModule, _itemFactory, InputItemKey, OutItemKey);
            _kitchenProductInventoryModule = new KitchenProductInventoryModule(_kitchenDefaultSetting.kitchenInventory, _kitchenDefaultSetting.kitchenInventory, _kitchenStatsModule, _itemFactory, OutItemKey, OutItemKey);
            _kitchenProductModule = new KitchenProductModule(_kitchenDefaultSetting.kitchenFactory, _kitchenDefaultSetting.kitchenFactory, _kitchenStatsModule, _kitchenMaterialInventoryModule, _kitchenProductInventoryModule, InputItemKey, OutItemKey);
            
            _kitchenModel = new KitchenModel();
            _kitchenViewModel = new KitchenViewModel(_kitchenModel);
            _kitchenDefaultSetting.kitchenView.BindViewModel(_kitchenViewModel);
            
            _interactionTrade = _kitchenDefaultSetting.InteractionTrade;
            _interactionTrade.RegisterReference(_kitchenDefaultSetting.kitchenFactory, _kitchenMaterialInventoryModule, _kitchenProductInventoryModule, InputItemKey);
            
            _kitchenProductModule.OnProcessingChanged += OnProcessingStateChanged;
            _kitchenProductModule.OnElapsedTimeChanged += UpdateViewModel;
        }
        
        private void OnProcessingStateChanged(bool isProcessing)
        {
            _kitchenDefaultSetting.kitchenView.gameObject.SetActive(isProcessing);
        }
        
        public override void Initialize() 
        {
            
        }

        private void UpdateViewModel()
        {
            var remainedMaterialCount = _kitchenMaterialInventoryModule.GetItemCount(InputItemKey);
            var elapsedTime = _kitchenProductModule.ElapsedTime;
            var productLeadTime = _kitchenProductModule.ProductLeadTime;
            _kitchenViewModel.UpdateValues(remainedMaterialCount, elapsedTime, productLeadTime);
        }

        private void Update()
        {
#if UNITY_EDITOR
            // TODO : Test Scripts
            if (Input.GetKeyDown(KeyCode.W))
            {
                _kitchenProductInventoryModule.ReceiveItem(OutItemKey, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
            }
#endif
            
            _kitchenProductInventoryModule.Update();
            _kitchenProductModule.Product();
        }
    }
}