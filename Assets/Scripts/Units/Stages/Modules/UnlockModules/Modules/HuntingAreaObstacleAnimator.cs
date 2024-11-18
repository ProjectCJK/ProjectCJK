using System;
using UnityEngine;

namespace Units.Stages.Modules.UnlockModules.Modules
{
    public class HuntingAreaObstacleAnimator : MonoBehaviour
    {
        private Animator Animator;
        private Action _onComplete;
        
        private void OnEnable()
        {
            if (Animator != null) Animator.GetComponent<Animator>();
        }

        public void SetBool(int isActive, bool value, Action onComplete)
        {
            Animator.SetBool(isActive, value);

            _onComplete = onComplete;
        }

        public void IsAnimationEnded()
        {
            _onComplete?.Invoke();
        }
    }
}