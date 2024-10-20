using Modules.DesignPatterns.FSMs.Interfaces;

namespace Modules.DesignPatterns.FSMs.Abstract
{
    public abstract class BaseStateMachine
    {
        private IState _currentState;

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();

            _currentState = newState;

            _currentState?.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }
        
        public void LateUpdate()
        {
            _currentState?.LateUpdate();
        }
    }
}