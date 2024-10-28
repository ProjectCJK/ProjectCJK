using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Modules.InventoryModules.Interfaces;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Abstract
{
    public interface IInventoryModule : IInitializable, IItemReceiver
    {
        event Action OnInventoryCountChanged;
        IItemController ItemController { get; }
        int MaxInventorySize { get; }
        int CurrentInventorySize { get; }
        void Update();
    }

    public interface IInventoryProperty
    {
        int MaxProductInventorySize { get; }
    }

    public abstract class InventoryModule : IInventoryModule
    {
        public event Action OnInventoryCountChanged;

        public abstract IItemController ItemController { get; }
        public abstract int MaxInventorySize { get; }
        public abstract Transform SenderTransform { get; }
        public abstract Transform ReceiverTransform { get; }

        public int CurrentInventorySize => Inventory.Values.Sum();
        
        protected readonly Dictionary<Tuple<EMaterialType, EItemType>, int> Inventory = new();
        protected readonly Stack<IItem> _spawnedItemStack = new();
        
        private const float SendItemInterval = 0.2f;
        private float _lastSendTime;

        public abstract void Initialize();
        public void Update() => TrySendItem();

        protected abstract void SendItem();

        /// <summary>
        /// 내부적으로 아이템을 처리하는 메서드. 콜백을 사용하여 아이템이 수신된 후의 추가 작업을 정의.
        /// </summary>
        private bool TransferItem(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition)
        {
            if (!CanReceiveItem()) return false;

            IItem item = ItemController.GetItem(inputItemKey, currentSenderPosition);

            // 아이템을 전송하고, 이후의 행동을 콜백으로 처리
            item.Transfer(currentSenderPosition, ReceiverTransform.position, () => OnItemReceived(inputItemKey, item));

            return true;
        }
        
        protected abstract void OnItemReceived(Tuple<EMaterialType, EItemType> inputItemKey, IItem item);
        
        private bool TransferItem(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition, Vector3 targetReceiverPosition)
        {
            if (!CanReceiveItem()) return false;

            IItem item = ItemController.GetItem(inputItemKey, currentSenderPosition);

            // 아이템을 전송하고, 이후의 행동을 콜백으로 처리
            item.Transfer(currentSenderPosition, targetReceiverPosition, () => OnItemReceived(inputItemKey, item));

            return true;
        }

        /// <summary>
        /// 아이템을 받은 후 파괴하는 메서드
        /// </summary>
        public bool ReceiveItem(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition)
        {
            return TransferItem(inputItemKey, currentSenderPosition);
        }
        
        public bool ReceiveItem(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition, Vector3 targetReceiverPosition)
        {
            return TransferItem(inputItemKey, currentSenderPosition, targetReceiverPosition);
        }
        
        // public bool ReceiveItem(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition, Vector3 targetReceiverPosition)
        // {
        //     return TransferItem(inputItemKey, currentSenderPosition, targetReceiverPosition, item =>
        //     {
        //         AddItem(inputItemKey);
        //         ItemController.ReturnItem(item);
        //         
        //         AddItem(inputItemKey);
        //         PushSpawnedItem(item);
        //     });
        // }

        /// <summary>
        /// 아이템을 제거하는 메서드
        /// </summary>
        public void RemoveItem(Tuple<EMaterialType, EItemType> itemKey)
        {
            if (!Inventory.ContainsKey(itemKey)) return;

            Inventory[itemKey]--;
            if (Inventory[itemKey] <= 0)
            {
                Inventory.Remove(itemKey);
            }
            OnInventoryCountChanged?.Invoke();
        }

        /// <summary>
        /// 아이템을 추가하는 메서드
        /// </summary>
        public void AddItem(Tuple<EMaterialType, EItemType> itemKey)
        {
            if (!Inventory.TryAdd(itemKey, 1))
            {
                Inventory[itemKey]++;
            }
            OnInventoryCountChanged?.Invoke();
        }

        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> inventoryKey) => Inventory.ContainsKey(inventoryKey);
        public bool CanReceiveItem() => CurrentInventorySize + 1 <= MaxInventorySize;

        protected bool IsInventoryFull() => CurrentInventorySize >= MaxInventorySize;
        protected bool IsReadyToSend() => Time.time >= _lastSendTime + SendItemInterval;
        protected void SetLastSendTime() => _lastSendTime = Time.time;

        private void TrySendItem()
        {
            if (IsReadyToSend())
            {
                SendItem();
                SetLastSendTime();
            }
        }
        
        protected void PushSpawnedItem(IItem item) => _spawnedItemStack.Push(item);
        protected IItem PopSpawnedItem() => _spawnedItemStack.Pop();
    }
}