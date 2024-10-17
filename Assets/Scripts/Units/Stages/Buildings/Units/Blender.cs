using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.InventoryModules.Units;
using Units.Modules.ProductModules;
using Units.Modules.StatsModules.Units;
using Units.Stages.Buildings.Abstract;
using Units.Stages.Buildings.Enums;
using Units.Stages.Buildings.Modules;
using Units.Stages.Buildings.UI;
using Units.Stages.Controllers;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Stages.Buildings.Units
{
    public interface IBlender : IRegisterReference<BuildingDataSO, IItemController>
    {
        
    }
    
    public class Blender : Building, IBlender
    {
        [SerializeField] private BlenderProductView _blenderProductView;
        
        public override EBuildingType BuildingType { get; protected set; }
        public override List<Tuple<EMaterialType, EProductType>> InputItemKey { get; protected set; }
        public override List<Tuple<EMaterialType, EProductType>> OutItemKey { get; protected set; }
        
        private IBuildingStatsModule _buildingStatsModule;
        private IBlenderInventoryModule _blenderInventoryModule;
        private IBuildingProductModule _buildingProductModule;
        private IItemController _itemController;
        private IInteractionTrade _interactionTrade;
        
        private BlenderProductViewModel _blenderProductViewModel;
        private BlenderProductModel _blenderProductModel;

        public void RegisterReference(BuildingDataSO buildingDataSo, IItemController itemController)
        {
            BuildingType = buildingDataSo.BuildingType;
            _itemController = itemController;

            InputItemKey = new List<Tuple<EMaterialType, EProductType>>();
            OutItemKey = new List<Tuple<EMaterialType, EProductType>>();
            
            InputItemKey.Add(new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.inputProductType));
            OutItemKey.Add(new Tuple<EMaterialType, EProductType>(buildingDataSo.MaterialType, buildingDataSo.outputProductType));
            
            _buildingStatsModule = new BuildingStatsModule(buildingDataSo);
            _blenderInventoryModule = new BlenderInventoryModule(transform, transform, _buildingStatsModule, _itemController, InputItemKey, OutItemKey);
            _buildingProductModule = new BuildingProductModule(_buildingStatsModule, _blenderInventoryModule, InputItemKey, OutItemKey);
            
            _blenderProductModel = new BlenderProductModel();
            _blenderProductViewModel = new BlenderProductViewModel(_blenderProductModel);
            _blenderProductView.BindViewModel(_blenderProductViewModel);
            
            _interactionTrade = GetComponentInChildren<InteractionTrade>();
            _interactionTrade.RegisterReference(_blenderInventoryModule, InputItemKey);
            
            _buildingProductModule.OnProcessingChanged += OnProcessingStateChanged;
            _buildingProductModule.OnElapsedTimeChanged += UpdateViewModel;
        }
        
        private void OnProcessingStateChanged(bool isProcessing)
        {
            _blenderProductView.gameObject.SetActive(isProcessing);
        }
        
        public override void Initialize() 
        {
            
        }

        private void UpdateViewModel()
        {
            var remainedMaterialCount = _blenderInventoryModule.GetItemCount(InputItemKey[0]);
            var elapsedTime = _buildingProductModule.ElapsedTime;
            var productLeadTime = _buildingProductModule.ProductLeadTime;
            _blenderProductViewModel.UpdateValues(remainedMaterialCount, elapsedTime, productLeadTime);
        }

        private void Update()
        {
            //TODO : Test Scripts
            if (Input.GetKeyDown(KeyCode.W))
            {
                _blenderInventoryModule.ReceiveItem(new Tuple<EMaterialType, EProductType>(EMaterialType.A, EProductType.Product));
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
            
            _blenderInventoryModule.SendItem();
            _buildingProductModule.Product();
        }
    }
}