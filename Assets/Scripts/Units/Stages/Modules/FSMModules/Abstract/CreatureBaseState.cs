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

        protected virtual void StartAnimationWithBool(int animationHash)
        {
            Creature.Animator.SetBool(animationHash, true);
        }

        protected virtual void StopAnimationWithBool(int animationHash)
        {
            Creature.Animator.SetBool(animationHash, false);
        }
    }
}