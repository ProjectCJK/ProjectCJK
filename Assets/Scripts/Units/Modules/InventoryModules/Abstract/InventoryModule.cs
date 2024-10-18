using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Modules.InventoryModules.Interfaces;
using Units.Stages.Items.Enums;
using UnityEditor;
using UnityEngine;

namespace Units.Modules.InventoryModules.Abstract
{
    public interface IInventoryModule : IInitializable, IItemReceiver
    {
        public event Action OnInventoryCountChanged;
        public int MaxInventorySize { get; }
        public int CurrentInventorySize { get; }
        public void SendItem();
    }

    public interface IInventoryProperty
    {
        public int MaxInventorySize { get; }
    }

    public abstract class InventoryModule : IInventoryModule
    {
        public event Action OnInventoryCountChanged;
        
        public abstract int MaxInventorySize { get; }
        public abstract Transform ReceiverTransform { get; }
        
        public int CurrentInventorySize => Inventory.Sum(item => item.Value);
        
        protected readonly Dictionary<Tuple<EMaterialType, EProductType>, int> Inventory = new();
        
        private const float SendItemInterval = 0.2f;
        private float _lastSendTime;

        public abstract void Initialize();

        public abstract void SendItem();
        
        public void RemoveItem(Tuple<EMaterialType, EProductType> itemKey)
        {
            Inventory[itemKey]--;
            
            OnInventoryCountChanged?.Invoke();
            
            if (Inventory[itemKey] == 0)
            {
                Inventory.Remove(itemKey);
            }
        }

        public void ReceiveItem(Tuple<EMaterialType, EProductType> itemKey)
        {
            if (CanReceiveItem()) AddItem(itemKey);
        }
        
        public void AddItem(Tuple<EMaterialType, EProductType> itemKey)
        {
            if (!Inventory.TryAdd(itemKey, 1)) Inventory[itemKey]++;
            
            OnInventoryCountChanged?.Invoke();
        }

        public bool HasMatchingItem(Tuple<EMaterialType, EProductType> InventoryKey) => Inventory.TryGetValue(InventoryKey, out _);
        public bool CanReceiveItem() => CurrentInventorySize + 1 <= MaxInventorySize;
        public bool IsInventoryFull() => CurrentInventorySize >= MaxInventorySize;
        protected bool IsReadyToSend() => Time.time >= _lastSendTime + SendItemInterval;
        protected void SetLastSendTime() => _lastSendTime = Time.time;
    }
}