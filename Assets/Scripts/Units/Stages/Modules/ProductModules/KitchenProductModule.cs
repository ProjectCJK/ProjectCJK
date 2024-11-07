using System;
using Interfaces;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using UnityEngine;

namespace Units.Stages.Modules.ProductModules.Abstract
{
    public interface IProductProperty
    {
        public float BaseProductLeadTime { get; }
    }
    
    public interface IKitchenProductModule : IInitializable
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
    
    public class KitchenProductModule : IKitchenProductModule
    {
        public event Action OnElapsedTimeChanged;
        public event Action<bool> OnProcessingChanged;
        
        public bool IsProcessing { get; private set; }
        public float ElapsedTime { get; private set; }
        public float ProductLeadTime => _kitchenStatsModule.CurrentKitchenOption2Value;

        private readonly KitchenStatsModule _kitchenStatsModule;
        private readonly KitchenMaterialInventoryModule _kitchenMaterialInventoryModule;
        private readonly KitchenProductInventoryModule _kitchenProductInventoryModule;

        private readonly string _inputItemKey;
        private readonly string _outputItemKey;

        public Transform SenderTransform { get; }
        public Transform ReceiverTransform { get; }

        public KitchenProductModule(Transform senderTransform,
            Transform receiverTransform,
            KitchenStatsModule kitchenStatsModule,
            KitchenMaterialInventoryModule kitchenMaterialInventoryModule,
            KitchenProductInventoryModule kitchenProductInventoryModule,
            string inputItemKey,
            string outputItemKey)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            _kitchenStatsModule = kitchenStatsModule;
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
            Debug.Log($"{ProductLeadTime}");
            if (IsProcessing)
            {
                if (IsProductProcessed())
                {
                    if (_kitchenProductInventoryModule.CanReceiveItem())
                    {
                        _kitchenProductInventoryModule.ReceiveItemThroughTransfer(_outputItemKey, 1, SenderTransform.position);
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
            return ElapsedTime >= ProductLeadTime;
        }

        private bool HasMatchingItem() => _kitchenMaterialInventoryModule.HasMatchingItem(_inputItemKey);

        private void SetProcessingFlag(bool isProcessing)
        {
            IsProcessing = isProcessing;
            OnProcessingChanged?.Invoke(isProcessing);
        }
    }
}