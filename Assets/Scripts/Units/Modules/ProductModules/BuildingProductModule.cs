using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Modules.InventoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Modules.ProductModules
{
    public interface IProductProperty
    {
        public float ProductLeadTime { get; }
    }
    
    public interface IBuildingProductModule : IInitializable
    {
        public event Action OnElapsedTimeChanged;
        public event Action<bool> OnProcessingChanged;
        
        public bool IsProcessing { get; }
        public float ElapsedTime { get; }
        public float ProductLeadTime { get; }

        public void Product();
    }
    
    public class BuildingProductModule : IBuildingProductModule
    {
        public event Action OnElapsedTimeChanged;
        public event Action<bool> OnProcessingChanged;
        
        public bool IsProcessing { get; private set; }
        public float ElapsedTime { get; private set; }

        public float ProductLeadTime => _productLeadTime;
        
        private readonly IBuildingStatsModule _buildingStatsModule;
        private readonly IBuildingInventoryModule _buildingInventoryModule;
        
        private readonly List<Tuple<EMaterialType, EItemType>> _inputItemKey;
        private readonly List<Tuple<EMaterialType, EItemType>> _outputItemKey;
        
        private float _productLeadTime => _buildingStatsModule.ProductLeadTime;
        private Tuple<EMaterialType, EItemType> _currentInputKey;
        private Tuple<EMaterialType, EItemType> _currentOutputKey;

        public BuildingProductModule(IBuildingStatsModule buildingStatsModule, IBuildingInventoryModule buildingInventoryModule, List<Tuple<EMaterialType, EItemType>> inputItemKey, List<Tuple<EMaterialType, EItemType>> outputItemKey)
        {
            _buildingStatsModule = buildingStatsModule;
            _buildingInventoryModule = buildingInventoryModule;
            _inputItemKey = inputItemKey;
            _outputItemKey = outputItemKey;
        }

        public void Initialize()
        {
            ElapsedTime = 0f;
            OnElapsedTimeChanged?.Invoke();
        }
        
        public void Product()
        {
            if (IsProcessing)
            {
                if (IsProductProcessed())
                {
                    _buildingInventoryModule.RemoveItem(_currentInputKey);
                    _buildingInventoryModule.AddItem(_currentOutputKey);

                    if (HasMatchingItem())
                    {
                        Initialize();
                    }
                    else
                    {
                        _currentInputKey = null;
                        SetProcessingFlag(false);
                    }
                }
            }
            else
            {
                if (HasMatchingItem())
                {
                    Initialize();
                    SetProcessingFlag(true);
                }
            }
        }

        private bool IsProductProcessed()
        {
            ElapsedTime += Time.deltaTime;
            OnElapsedTimeChanged?.Invoke();
            return ElapsedTime >= _productLeadTime;
        }

        private bool HasMatchingItem()
        {
            foreach (Tuple<EMaterialType, EItemType> currentInputKey in _inputItemKey.Where(currentInputKey => _buildingInventoryModule.HasMatchingItem(currentInputKey)))
            {
                _currentInputKey = currentInputKey;
                _currentOutputKey = new Tuple<EMaterialType, EItemType>(currentInputKey.Item1, EItemType.Product);
                return true;
            }
            return false;
        }

        private void SetProcessingFlag(bool isProcessing)
        {
            IsProcessing = isProcessing;
            OnProcessingChanged?.Invoke(isProcessing);
        }
    }
}