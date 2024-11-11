using Modules.DesignPatterns.FSMs.Abstract;

namespace Units.Stages.Modules.FSMModules.Units.Creature
{
    public class CreatureStateMachine : BaseStateMachine
    {
        public CreatureStateMachine(Stages.Units.Creatures.Abstract.Creature creature)
        {
            Creature = creature;

            CreatureIdleState = new CreatureIdleState(this);
            CreatureRunState = new CreatureRunState(this);
        }

        public Stages.Units.Creatures.Abstract.Creature Creature { get; private set; }

        public CreatureIdleState CreatureIdleState { get; private set; }
        public CreatureRunState CreatureRunState { get; private set; }
    }
}