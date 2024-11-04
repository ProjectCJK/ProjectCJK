using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Units
{
    public interface IGuestSpawnZone : ISpawnZone
    {
        public ENPCType NpcType { get; }
    }
    
    public class GuestSpawnZone : SpawnZone, IGuestSpawnZone
    {
        public ECreatureType CreatureType => ECreatureType.NPC;
        public ENPCType NpcType => ENPCType.Guest;
    }
}