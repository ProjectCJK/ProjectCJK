using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.InventoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.ProductModules;
using Units.Modules.StatsModules.Units;
using Units.Stages.Buildings.Abstract;
using Units.Stages.Buildings.Enums;
using Units.Stages.Buildings.Modules;
using Units.Stages.Buildings.UI;
using Units.Stages.Buildings.UI.Kitchens;
using Units.Stages.Controllers;
using Units.Stages.Items.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Stages.Buildings.Units
{
    public interface IKitchen : IBuilding, IRegisterReference<IItemController>
    {
        
    }

    [Serializable]
    public struct KitchenDefaultSetting
    {
        [Header("DataSo")]
        public BuildingDataSO buildingDataSo;
        
        [Header("Kitchen UI")]
        public KitchenView kitchenView;
    }

    [Serializable]
    public struct KitchenCustomSetting
    {
        [Header("재료 타입")]
        public EMaterialType MaterialType;
        
        [Header("Input 아이템 타입")]
        public EItemType InputItemType;
        
        [Header("Output 아이템 타입")]
        public EItemType OutputItemType;
    }
    
    public class Kitchen : Building, IKitchen
    {
        public override EBuildingType BuildingType => KitchenDefaultSetting.buildingDataSo.BuildingType;
        public override List<Tuple<EMaterialType, EItemType>> InputItemKey { get; protected set; }
        public override List<Tuple<EMaterialType, EItemType>> OutItemKey { get; protected set; }
     
        public KitchenDefaultSetting KitchenDefaultSetting;
        public KitchenCustomSetting kitchenCustomSetting;
        
        private IBuildingStatsModule _buildingStatsModule;
        private IKitchenInventoryModule _kitchenInventoryModule;
        private IBuildingProductModule _buildingProductModule;
        private IItemController _itemController;
        private IInteractionTrade _interactionTrade;
        
        private KitchenViewModel _kitchenViewModel;
        private KitchenModel _kitchenModel;

        public void RegisterReference(IItemController itemController)
        {
            _itemController = itemController;

            InputItemKey = new List<Tuple<EMaterialType, EItemType>>();
            OutItemKey = new List<Tuple<EMaterialType, EItemType>>();
            
            InputItemKey.Add(new Tuple<EMaterialType, EItemType>(kitchenCustomSetting.MaterialType, kitchenCustomSetting.InputItemType));
            OutItemKey.Add(new Tuple<EMaterialType, EItemType>(kitchenCustomSetting.MaterialType, kitchenCustomSetting.OutputItemType));
            
            _buildingStatsModule = new BuildingStatsModule(KitchenDefaultSetting.buildingDataSo);
            _kitchenInventoryModule = new KitchenInventoryModule(transform, transform, _buildingStatsModule, _itemController, InputItemKey, OutItemKey);
            _buildingProductModule = new BuildingProductModule(_buildingStatsModule, _kitchenInventoryModule, InputItemKey, OutItemKey);
            
            _kitchenModel = new KitchenModel();
            _kitchenViewModel = new KitchenViewModel(_kitchenModel);
            KitchenDefaultSetting.kitchenView.BindViewModel(_kitchenViewModel);
            
            _interactionTrade = GetComponentInChildren<InteractionTrade>();
            _interactionTrade.RegisterReference(_kitchenInventoryModule, InputItemKey);
            
            _buildingProductModule.OnProcessingChanged += OnProcessingStateChanged;
            _buildingProductModule.OnElapsedTimeChanged += UpdateViewModel;
        }
        
        private void OnProcessingStateChanged(bool isProcessing)
        {
            KitchenDefaultSetting.kitchenView.gameObject.SetActive(isProcessing);
        }
        
        public override void Initialize() 
        {
            
        }

        private void UpdateViewModel()
        {
            var remainedMaterialCount = _kitchenInventoryModule.GetItemCount(InputItemKey[0]);
            var elapsedTime = _buildingProductModule.ElapsedTime;
            var productLeadTime = _buildingProductModule.ProductLeadTime;
            _kitchenViewModel.UpdateValues(remainedMaterialCount, elapsedTime, productLeadTime);
        }

        private void Update()
        {
            //TODO : Test Scripts
            if (Input.GetKeyDown(KeyCode.W))
            {
                _kitchenInventoryModule.ReceiveItem(new Tuple<EMaterialType, EItemType>(EMaterialType.A, EItemType.Product));
            }
            
            // var remainedMaterialCount = _blenderInventoryModule.GetItemCount(InputItemKey);
            // var elapsedTime = _buildingProductModule.ElapsedTime;
            // var productLeadTime = _buildingProductModule.ProductLeadTime;
            // _blenderProductViewModel.UpdateValues(remainedMaterialCount, elapsedTime, productLeadTime);
            //
            // // 제품 생산 중인지 확인
            // if (_buildingProductModule.IsProcessing)
            // {
            //     // 생산 중이라면 View 활성화
            //     if (!_blenderProductView.gameObject.activeSelf)
            //     {
            //         _blenderProductView.gameObject.SetActive(true);
            //     }
            // }
            // else
            // {
            //     // 생산 중이 아니라면 View 비활성화
            //     if (_blenderProductView.gameObject.activeSelf)
            //     {
            //         _blenderProductView.gameObject.SetActive(false);
            //     }
            // }
            
            _kitchenInventoryModule.SendItem();
            _buildingProductModule.Product();
        }
    }
}