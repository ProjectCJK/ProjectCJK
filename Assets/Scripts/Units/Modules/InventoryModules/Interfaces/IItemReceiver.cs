using System;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Interfaces
{
    public interface IItemReceiver
    {
        public Transform SenderTransform { get; }
        public Transform ReceiverTransform { get; }
        
        public bool ReceiveItemWithDestroy(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition);
        public bool ReceiveItemWithDestroy(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition, Vector3 targetReceiverPosition);
        public bool ReceiveItemWithoutDestroy(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition);
        public bool ReceiveItemWithoutDestroy(Tuple<EMaterialType, EItemType> inputItemKey, Vector3 currentSenderPosition, Vector3 targetReceiverPosition);
        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey);
        public bool CanReceiveItem();
    }
}