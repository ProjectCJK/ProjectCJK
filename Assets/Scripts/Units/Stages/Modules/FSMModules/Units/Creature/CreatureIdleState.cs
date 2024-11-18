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
            base.Enter();
            StartAnimationWithBool(AnimationParameterData.IdleParameterHash);
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
            StopAnimationWithBool(AnimationParameterData.IdleParameterHash);
        }
    }
}