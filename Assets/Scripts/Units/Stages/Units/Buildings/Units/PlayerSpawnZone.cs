using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IPlayerSpawnZone : ISpawnZone
    {
    }

    public class PlayerSpawnZone : SpawnZone, IPlayerSpawnZone
    {
        public ECreatureType CreatureType => ECreatureType.Player;
    }
}