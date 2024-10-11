using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Interfaces;
using Units.Modules.InventoryModules.Interfaces;
using UnityEngine;

namespace Units.Modules.InventoryModules.Abstract
{
    public interface IInventoryModule : IInitializable, IItemReceiver
    {
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
        public abstract int MaxInventorySize { get; }
        public Tuple<EMaterialType, EItemType> InputItemKey { get; protected set; }
        public int CurrentInventorySize => Inventory.Sum(item => item.Value);
        
        protected readonly Dictionary<Tuple<EMaterialType, EItemType>, int> Inventory = new();

        public abstract void Initialize();
        
        public abstract void SendItem();

        #region 아이템 전송
        
        public void RemoveItem(Tuple<EMaterialType, EItemType> itemKey)
        {
            Inventory[itemKey]--;

            if (Inventory[itemKey] == 0)
            {
                Inventory.Remove(itemKey);
            }
        }

        #endregion
        
        #region 아이템 수신

        public void ReceiveItem(Tuple<EMaterialType, EItemType> itemKey)
        {
            if (CanReceiveItem()) AddItem(itemKey);
        }
        
        public void AddItem(Tuple<EMaterialType, EItemType> itemKey)
        {
            if (!Inventory.TryAdd(itemKey, 1)) Inventory[itemKey]++;

            Debug.Log($"{this} Inventory => 종류 {Inventory.Count}개 / 전체 개수 {CurrentInventorySize}");

            Debug.Log(Inventory.TryGetValue(itemKey, out var count)
                ? $"{this} Inventory[{itemKey}] => {count}"
                : $"{this} Inventory[{itemKey}] => null");
        }
        
        #endregion

        #region 아이템 송/수신 체크

        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey) => Inventory.TryGetValue(InventoryKey, out _);
        public bool CanReceiveItem() => CurrentInventorySize + 1 <= MaxInventorySize;
        public bool IsInventoryFull() => CurrentInventorySize >= MaxInventorySize;

        #endregion
    }
}