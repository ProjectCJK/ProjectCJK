using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Modules.InventoryModules.Interfaces
{
    public interface ICreatureItemReceiver : IItemReceiver
    {
        public ECreatureType CreatureType { get; }
        public void RegisterItemReceiver(IInteractionTrade interactionZone, bool isConnected);
    }
}