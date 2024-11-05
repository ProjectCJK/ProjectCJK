using Modules.DesignPatterns.FSMs.Modules;
using Units.Stages.Modules.FSMModules.Abstract;

namespace Units.Stages.Modules.FSMModules.Units.Creature
{
    public class CreatureRunState : CreatureBaseState
    {
        public CreatureRunState(CreatureStateMachine creatureStateMachine) : base(creatureStateMachine)
        {
        }

        public override void Enter()
        {
            StartAnimationWithBool(AnimationParameterData.RunParameterHash);
        }

        public override void Update() { }

        public override void FixedUpdate() { }

        public override void LateUpdate() { }

        public override void Exit()
        {
            StopAnimationWithBool(AnimationParameterData.RunParameterHash);
        }
    }
}