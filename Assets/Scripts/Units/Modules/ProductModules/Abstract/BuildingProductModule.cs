using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Modules.InventoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
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
        
        public Transform SenderTransform { get; }
        public Transform ReceiverTransform { get; }
        
        public bool IsProcessing { get; }
        public float ElapsedTime { get; }
        public float ProductLeadTime { get; }

        public void Product();
    }
    
    public abstract class BuildingProductModule : IBuildingProductModule
    {
        public event Action OnElapsedTimeChanged;
        public event Action<bool> OnProcessingChanged;
        
        public bool IsProcessing { get; private set; }
        public float ElapsedTime { get; private set; }

        public float ProductLeadTime => _productLeadTime;

        private readonly IBuildingStatsModule _buildingStatsModule;
        private readonly IKitchenInventoryModule _kitchenInventoryModule;
        
        private readonly Tuple<EMaterialType, EItemType> _inputItemKey;
        private readonly Tuple<EMaterialType, EItemType> _outputItemKey;

        private float _productLeadTime => _buildingStatsModule.ProductLeadTime;

        public Transform SenderTransform { get; }
        public Transform ReceiverTransform { get; }

        protected BuildingProductModule(
            Transform senderTransform,
            Transform receiverTransform,
            IBuildingStatsModule buildingStatsModule,
            IKitchenInventoryModule kitchenInventoryModule,
            Tuple<EMaterialType, EItemType> inputItemKey,
            Tuple<EMaterialType, EItemType> outputItemKey)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            _buildingStatsModule = buildingStatsModule;
            _kitchenInventoryModule = kitchenInventoryModule;
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
                    if (_kitchenInventoryModule.ReceiveItemWithoutDestroy(_outputItemKey, SenderTransform.position))
                    {
                        _kitchenInventoryModule.RemoveItem(_inputItemKey);       
                    }
                    
                    if (HasMatchingItem())
                    {
                        Initialize();
                    }
                    else
                    {
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

        private bool HasMatchingItem() => _kitchenInventoryModule.HasMatchingItem(_inputItemKey);

        private void SetProcessingFlag(bool isProcessing)
        {
            IsProcessing = isProcessing;
            OnProcessingChanged?.Invoke(isProcessing);
        }
    }
}