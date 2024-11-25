using Modules.DesignPatterns.FSMs.Abstract;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Units.Creatures.Abstract;
using UnityEngine;

namespace Units.Stages.Modules.FSMModules.Abstract
{
    public abstract class CreatureBaseState : BaseState
    {
        protected readonly Creature Creature;
        protected readonly CreatureStateMachine CreatureStateMachine;

        protected CreatureBaseState(CreatureStateMachine creatureStateMachine)
        {
            CreatureStateMachine = creatureStateMachine;
            Creature = CreatureStateMachine.Creature;
        }

        protected override Animator Animator => Creature.Animator;

        public override void Enter() { }
        
        public override void Update() { }
        
        public override void FixedUpdate() { }
        
        public override void LateUpdate() { }
        
        public override void Exit() { }

        protected void StartAnimationWithBool(int animationHash)
        {
            if (Animator == null) return;
            Creature.Animator.SetBool(animationHash, true);
        }

        protected void StopAnimationWithBool(int animationHash)
        {
            if (Animator == null) return;
            Creature.Animator.SetBool(animationHash, false);
        }
    }
}