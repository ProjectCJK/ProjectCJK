using Units.Stages.Modules.FSMModules.Units.Creature;

namespace Units.Stages.Modules.FSMModules.Units.Player
{
    public class PlayerStateMachine : CreatureStateMachine
    {
        public PlayerIdleState PlayerIdleState { get; private set; }
        public PlayerRunState PlayerRunState { get; private set; }
        
        public PlayerStateMachine(Stages.Units.Creatures.Abstract.Creature creature) : base(creature)
        {
            PlayerIdleState = new PlayerIdleState(this);
            PlayerRunState = new PlayerRunState(this);
        }
    }
}