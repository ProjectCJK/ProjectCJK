using Units.Stages.Modules.FSMModules.Units.Creature;

namespace Units.Stages.Modules.FSMModules.Units.Player
{
    public class PlayerIdleState : CreatureIdleState
    {
        public PlayerIdleState(CreatureStateMachine creatureStateMachine) : base(creatureStateMachine)
        {
        }
    }
}