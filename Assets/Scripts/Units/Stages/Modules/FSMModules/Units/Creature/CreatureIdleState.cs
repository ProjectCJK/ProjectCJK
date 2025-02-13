using Modules.DesignPatterns.FSMs.Modules;
using Units.Stages.Modules.FSMModules.Abstract;

namespace Units.Stages.Modules.FSMModules.Units.Creature
{
    public class CreatureIdleState : CreatureBaseState
    {
        public CreatureIdleState(CreatureStateMachine creatureStateMachine) : base(creatureStateMachine)
        {
        }

        public override void Enter()
        {
            StartAnimationWithBool(AnimationParameterData.IdleParameterHash);
        }

        public override void Exit()
        {
            StopAnimationWithBool(AnimationParameterData.IdleParameterHash);
        }
    }
}