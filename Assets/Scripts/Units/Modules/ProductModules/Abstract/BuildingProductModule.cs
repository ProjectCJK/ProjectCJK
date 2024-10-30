using System;
using Interfaces;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.StatsModules.Abstract;
using Units.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Modules.ProductModules.Abstract
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
        private readonly IKitchenMaterialInventoryModule _kitchenMaterialInventoryModule;
        private readonly IKitchenProductInventoryModule _kitchenProductInventoryModule;

        private readonly string _inputItemKey;
        private readonly string _outputItemKey;

        private float _productLeadTime => _buildingStatsModule.ProductLeadTime;

        public Transform SenderTransform { get; }
        public Transform ReceiverTransform { get; }

        protected BuildingProductModule(Transform senderTransform,
            Transform receiverTransform,
            IBuildingStatsModule buildingStatsModule,
            IKitchenMaterialInventoryModule kitchenMaterialInventoryModule,
            IKitchenProductInventoryModule kitchenProductInventoryModule,
            string inputItemKey,
            string outputItemKey)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            _buildingStatsModule = buildingStatsModule;
            _kitchenMaterialInventoryModule = kitchenMaterialInventoryModule;
            _kitchenProductInventoryModule = kitchenProductInventoryModule;
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

                    if (_kitchenProductInventoryModule.CanReceiveItem())
                    {
                        _kitchenProductInventoryModule.ReceiveItem(_outputItemKey, SenderTransform.position);
                        _kitchenMaterialInventoryModule.RemoveItem(_inputItemKey);
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

        private bool HasMatchingItem() => _kitchenMaterialInventoryModule.HasMatchingItem(_inputItemKey);

        private void SetProcessingFlag(bool isProcessing)
        {
            IsProcessing = isProcessing;
            OnProcessingChanged?.Invoke(isProcessing);
        }
    }
}