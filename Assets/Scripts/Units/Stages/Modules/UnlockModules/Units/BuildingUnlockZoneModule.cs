using System;
using Managers;
using Units.Stages.Enums;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using UnityEngine;

namespace Units.Stages.Modules.UnlockModules.Units
{
    public class BuildingUnlockZoneModule : UnlockZoneModule
    {
        public GameObject TargetObject;
        public override EUnlockZoneType UnlockZoneType => EUnlockZoneType.Building;
        public override event Action<string, EActiveStatus> OnChangeActiveStatus;

        public override void SetCurrentState(EActiveStatus state)
        {
            switch (state)
            {
                case EActiveStatus.Active:
                    ActiveStatus = state;
                    if (_playerCollision != null && !_playerCollision.activeInHierarchy) _playerCollision.SetActive(false);
                    if (TargetObject != null && !TargetObject.activeInHierarchy) TargetObject.SetActive(true);
                    unlockZoneAnimator.SetBool(Lock, false);
                    unlockZoneAnimator.SetBool(Standby, false);
                    unlockZoneAnimator.SetBool(Unlock, true);
                    OnChangeActiveStatus?.Invoke(TargetKey, EActiveStatus.Active);
                    break;
                case EActiveStatus.Standby:
                    ActiveStatus = state;
                    if (_playerCollision != null && !_playerCollision.activeInHierarchy) _playerCollision.SetActive(true);
                    if (TargetObject != null && TargetObject.activeInHierarchy) TargetObject.SetActive(false);
                    unlockZoneAnimator.SetBool(Lock, false);
                    unlockZoneAnimator.SetBool(Standby, true);
                    unlockZoneAnimator.SetBool(Unlock, false);
                    break;
                case EActiveStatus.Lock:
                    ActiveStatus = state;
                    if (_playerCollision != null && !_playerCollision.activeInHierarchy) _playerCollision.SetActive(true);
                    if (TargetObject != null && TargetObject.activeInHierarchy) TargetObject.SetActive(false);
                    unlockZoneAnimator.SetBool(Lock, true);
                    unlockZoneAnimator.SetBool(Standby, false);
                    unlockZoneAnimator.SetBool(Unlock, false);
                    break;
            }
        }
    }
}