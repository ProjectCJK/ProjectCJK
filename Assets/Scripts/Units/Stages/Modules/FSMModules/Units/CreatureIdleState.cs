using Modules.DesignPatterns.FSMs.Modules;
using Units.Stages.Modules.FSMModules.Abstract;

namespace Units.Stages.Modules.FSMModules.Units
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

        public override void Update() { }

        public override void FixedUpdate() { }

        public override void LateUpdate() { }

        public override void Exit()
        {
            StopAnimationWithBool(AnimationParameterData.IdleParameterHash);
        }
    }
}