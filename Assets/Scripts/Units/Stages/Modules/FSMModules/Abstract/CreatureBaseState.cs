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

        public override void Enter()
        {
            if (Animator == null)
            {
                Debug.LogError($"{Creature.CreatureType} Animator is null");
                return;
            }
        }
        
        public override void Update()
        {
            if (Animator == null)
            {
                Debug.LogError($"{Creature.CreatureType} Animator is null");
                return;
            }
        }
        
        public override void FixedUpdate()
        {
            if (Animator == null)
            {
                Debug.LogError($"{Creature.CreatureType} Animator is null");
                return;
            }
        }
        
        public override void LateUpdate()
        {
            if (Animator == null)
            {
                Debug.LogError($"{Creature.CreatureType} Animator is null");
                return;
            }
        }
        
        public override void Exit()
        {
            if (Animator == null)
            {
                Debug.LogError($"{Creature.CreatureType} Animator is null");
                return;
            }
        }

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