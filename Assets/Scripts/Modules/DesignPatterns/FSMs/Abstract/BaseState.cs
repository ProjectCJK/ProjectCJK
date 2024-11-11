using Modules.DesignPatterns.FSMs.Interfaces;
using UnityEngine;

namespace Modules.DesignPatterns.FSMs.Abstract
{
    public abstract class BaseState : IState
    {
        protected abstract Animator Animator { get; }

        public abstract void Enter();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void LateUpdate();
        public abstract void Exit();
    }
}