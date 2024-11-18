using System;
using UnityEngine;

namespace Units.Stages.Modules.UnlockModules.Modules
{
    public class HuntingAreaObstacleAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private Action _onComplete;
        
        public void SetBool(int isActive, bool value, Action onComplete)
        {
            _animator.SetBool(isActive, value);

            _onComplete = onComplete;
        }

        public void IsAnimationEnded()
        {
            _onComplete?.Invoke();
        }
    }
}