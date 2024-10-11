using System;
using Enums;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units;
using Units.Modules.StatsModules.Units;
using UnityEngine;

namespace Units.Modules
{
    public interface IProductProperty
    {
        public float ProductLeadTime { get; }
    }
    
    public interface IBuildingProductModule : IRegisterReference<IBuildingStatsModule, IBuildingInventoryModule, Tuple<EMaterialType, EItemType>, Tuple<EMaterialType, EItemType>>, IInitializable
    {
        public void Product();
    }
    
    public class BuildingProductModule : IBuildingProductModule
    {
        private IBuildingStatsModule _buildingStatsModule;
        private IBuildingInventoryModule _buildingInventoryModule;
        
        private Tuple<EMaterialType, EItemType> _inputItemKey;
        private Tuple<EMaterialType, EItemType> _outputItemKey;
        
        private float _productLeadTime => _buildingStatsModule.ProductLeadTime;
        private float _elapsedTime;
        private bool isProcessing;
        
        public void RegisterReference(IBuildingStatsModule buildingStatsModule, IBuildingInventoryModule buildingInventoryModule, Tuple<EMaterialType, EItemType> inputItemKey, Tuple<EMaterialType, EItemType> outputItemKey)
        {
            _buildingStatsModule = buildingStatsModule;
            _buildingInventoryModule = buildingInventoryModule;
            _inputItemKey = inputItemKey;
            _outputItemKey = outputItemKey;
        }
        
        public void Initialize()
        {
            _elapsedTime = 0f;
            isProcessing = false;
        }
        
        public void Product()
        {
            if (isProcessing)
            {
                if (IsProductProcessed())
                {
                    Debug.Log($"{this} Material ({_inputItemKey}) 투입");
                    _buildingInventoryModule.RemoveItem(_inputItemKey);
                    
                    Debug.Log($"{this} Product ({_outputItemKey}) 생산 완료");
                    _buildingInventoryModule.AddItem(_outputItemKey);
                
                    Initialize();
                }
            }
            else
            {
                if (GetItem())
                {
                    isProcessing = true;
                }
            }
        }

        private bool IsProductProcessed()
        {
            _elapsedTime += Time.deltaTime;

            return _elapsedTime >= _productLeadTime;
        }

        private bool GetItem()
        {
            return _buildingInventoryModule.HasMatchingItem(_inputItemKey);
        }
    }
}