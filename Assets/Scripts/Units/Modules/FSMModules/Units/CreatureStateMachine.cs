using Modules.DesignPatterns.FSMs.Abstract;
using Units.Stages.Creatures.Abstract;

namespace Units.Modules.FSMModules.Units
{
    public class CreatureStateMachine : BaseStateMachine
    {
        public Creature Creature { get; private set; }
        
        public CreatureIdleState CreatureIdleState { get; private set; }
        public CreatureRunState CreatureRunState { get; private set; }
        
        public CreatureStateMachine(Creature creature)
        {
            Creature = creature;

            CreatureIdleState = new CreatureIdleState(this);
            CreatureRunState = new CreatureRunState(this);
        }
    }
}