using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
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