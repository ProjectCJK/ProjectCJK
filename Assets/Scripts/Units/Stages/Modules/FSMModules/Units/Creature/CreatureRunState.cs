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
            base.Enter();
            StartAnimationWithBool(AnimationParameterData.RunParameterHash);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimationWithBool(AnimationParameterData.RunParameterHash);
        }
    }
}