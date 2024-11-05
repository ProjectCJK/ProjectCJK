using Modules.DesignPatterns.FSMs.Abstract;

namespace Units.Stages.Modules.FSMModules.Units.Creature
{
    public class CreatureStateMachine : BaseStateMachine
    {
        public Stages.Units.Creatures.Abstract.Creature Creature { get; private set; }
        
        public CreatureStateMachine(Stages.Units.Creatures.Abstract.Creature creature)
        {
            Creature = creature;
        }
    }
}