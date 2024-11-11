using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Modules.InventoryModules.Interfaces
{
    public interface ICreatureItemReceiver : IItemReceiver
    {
        public ECreatureType CreatureType { get; }
        public void RegisterItemReceiver(ITradeZone zone, bool isConnected);
    }
}