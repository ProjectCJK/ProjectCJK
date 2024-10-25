using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.ProductModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Buildings.UI.Kitchens;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IKitchen : IBuilding, IRegisterReference<IItemController>
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
        
        public override EBuildingType BuildingType { get; protected set; }
        public override Tuple<EMaterialType, EItemType> InputItemKey { get; protected set; }
        public override Tuple<EMaterialType, EItemType> OutItemKey { get; protected set; }
        
        private IBuildingStatsModule _buildingStatsModule;
        private IKitchenInventoryModule _kitchenInventoryModule;
        private IKitchenProductModule _kitchenProductModule;
        private IItemController _itemController;
        private IInteractionTrade _interactionTrade;
        
        private KitchenViewModel _kitchenViewModel;
        private KitchenModel _kitchenModel;
        
        private BuildingDataSO _kitchenDataSO;

        public void RegisterReference(IItemController itemController)
        {
            _kitchenDataSO = DataManager.Instance.KitchenData;
            
            _itemController = itemController;

            InputItemKey = new Tuple<EMaterialType, EItemType>(_kitchenCustomSetting.MaterialType, _kitchenCustomSetting.InputItemType);
            OutItemKey = new Tuple<EMaterialType, EItemType>(_kitchenCustomSetting.MaterialType, _kitchenCustomSetting.OutputItemType);
            
            _buildingStatsModule = new BuildingStatsModule(_kitchenDataSO);
            _kitchenInventoryModule = new KitchenInventoryModule(_kitchenDefaultSetting.kitchenInventory, _kitchenDefaultSetting.kitchenInventory, _buildingStatsModule, _itemController, InputItemKey, OutItemKey);
            _kitchenProductModule = new KitchenProductModule(_kitchenDefaultSetting.kitchenFactory, _kitchenDefaultSetting.kitchenFactory, _buildingStatsModule, _kitchenInventoryModule, InputItemKey, OutItemKey);
            
            _kitchenModel = new KitchenModel();
            _kitchenViewModel = new KitchenViewModel(_kitchenModel);
            _kitchenDefaultSetting.kitchenView.BindViewModel(_kitchenViewModel);
            
            _interactionTrade = _kitchenDefaultSetting.InteractionTrade;
            _interactionTrade.RegisterReference(_kitchenDefaultSetting.kitchenFactory, _kitchenInventoryModule, InputItemKey);
            
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
            var remainedMaterialCount = _kitchenInventoryModule.GetItemCount(InputItemKey);
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
                _kitchenInventoryModule.ReceiveItemWithDestroy(OutItemKey, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
            }
#endif
            
            _kitchenInventoryModule.Update();
            _kitchenProductModule.Product();
        }
    }
}