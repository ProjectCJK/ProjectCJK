using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;

namespace Units.Stages.Modules.InventoryModules.Interfaces
{
    public interface ICreatureItemReceiver : IItemReceiver
    {
        public ECreatureType CreatureType { get; }
        public void RegisterItemReceiver(ITradeZone zone, bool isConnected);
    }
}