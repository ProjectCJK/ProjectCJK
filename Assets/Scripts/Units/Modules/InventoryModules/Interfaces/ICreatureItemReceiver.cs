using Units.Games.Creatures.Enums;
using UnityEngine;

namespace Units.Modules.InventoryModules.Interfaces
{
    public interface ICreatureItemReceiver : IItemReceiver
    {
        public ECreatureType CreatureType { get; }
        public void ConnectWithInteractionTradeZone(Transform interactionZone, bool isConnected);
    }
}