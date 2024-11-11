using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Units.Creatures.Abstract
{
    public interface INPCProperty
    {
        public ENPCType NPCType { get; }
    }

    public interface INPC : ICreature
    {
    }

    public abstract class NPC : Creature, INPC
    {
    }
}