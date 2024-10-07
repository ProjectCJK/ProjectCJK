using Units.Items.Abstract;
using UnityEngine;

namespace Units.Buildings.Interfaces
{
    public interface IItemReceiver
    {
        public void ReceiveItem(BaseItem item);
    }
}