using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Interfaces
{
    public interface IItemReceiver
    {
        public Transform SenderTransform { get; }
        public Transform ReceiverTransform { get; }

        public void ReceiveItemThroughTransfer(string inputItemKey, int count, Vector3 currentSenderPosition);
        public bool HasMatchingItem(string InventoryKey);
        public bool CanReceiveItem();
    }
}