using System;
using Units.Stages.Enums;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Modules;
using UnityEngine;

namespace Units.Stages.Modules.UnlockModules.Units
{
    public class HuntingZoneUnlockZoneModule : UnlockZoneModule
    {
        public override EUnlockZoneType UnlockZoneType => EUnlockZoneType.Hunting;
        public override event Action<string, EActiveStatus> OnChangeActiveStatus;
        
        private static readonly int IsActive = Animator.StringToHash("IsActive");

        public override void SetCurrentState(EActiveStatus state)
        {
            switch (state)
            {
                case EActiveStatus.Active:
                    ActiveStatus = state;
                    if (_playerCollision != null && _playerCollision.activeInHierarchy) _playerCollision.GetComponentInChildren<HuntingAreaObstacleAnimator>().SetBool(IsActive, false, IsAnimationEnded);
                    unlockZoneAnimator.SetBool(Lock, false);
                    unlockZoneAnimator.SetBool(Standby, false);
                    unlockZoneAnimator.SetBool(Unlock, true);
                    OnChangeActiveStatus?.Invoke(TargetKey, EActiveStatus.Active);
                    break;
                case EActiveStatus.Standby:
                    ActiveStatus = state;
                    if (_playerCollision != null && !_playerCollision.activeInHierarchy) _playerCollision.SetActive(true);
                    unlockZoneAnimator.SetBool(Lock, false);
                    unlockZoneAnimator.SetBool(Standby, true);
                    unlockZoneAnimator.SetBool(Unlock, false);
                    break;
                case EActiveStatus.Lock:
                    ActiveStatus = state;
                    if (_playerCollision != null && !_playerCollision.activeInHierarchy) _playerCollision.SetActive(true);
                    unlockZoneAnimator.SetBool(Lock, true);
                    unlockZoneAnimator.SetBool(Standby, false);
                    unlockZoneAnimator.SetBool(Unlock, false);
                    break;
            }
        }

        private void IsAnimationEnded()
        {
            _playerCollision.gameObject.SetActive(false);
        }
    }
}