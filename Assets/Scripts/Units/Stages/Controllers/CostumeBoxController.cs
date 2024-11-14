using System;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public class CostumeBoxController : MonoBehaviour
    {
        public Animator Animator { get; private set; }
        public bool IsEndedIdleAnimation { get; private set; }
        public bool IsEndedOpenIdleAnimation { get; set; }
        public bool IsEndedResultAnimation { get; set; }

        private void OnEnable()
        {
            if (Animator == null) Animator = GetComponent<Animator>();
            
            IsEndedIdleAnimation = false;
            IsEndedOpenIdleAnimation = false;
            IsEndedResultAnimation = false;
        }
        
        public void OnCostumeBoxIdleAnimationEnded() => IsEndedIdleAnimation = true;
        
        public void OnCostumeBoxOpenIdleAnimationEnded() => IsEndedOpenIdleAnimation = true;
        public void OnCostumeBoxResultAnimationEnded() => IsEndedResultAnimation = true;
    }
}
