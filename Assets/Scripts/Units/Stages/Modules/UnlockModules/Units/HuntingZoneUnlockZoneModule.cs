using System;
using Units.Stages.Enums;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using UnityEngine;

namespace Units.Stages.Modules.UnlockModules.Units
{
    public class HuntingZoneUnlockZoneModule : UnlockZoneModule
    {
        [SerializeField] private GameObject _playerCollision;
        public override EUnlockZoneType UnlockZoneType => EUnlockZoneType.Hunting;
        public override event Action<string, EActiveStatus> OnChangeActiveStatus;

        public override void SetCurrentState(EActiveStatus state)
        {
            switch (state)
            {
                case EActiveStatus.Active:
                    ActiveStatus = state;
                    OnChangeActiveStatus?.Invoke(TargetKey, EActiveStatus.Active);
                    if (_playerCollision != null && _playerCollision.activeInHierarchy)
                        _playerCollision.SetActive(false);
                    if (StandbyObject != null && StandbyObject.activeInHierarchy) StandbyObject.SetActive(false);
                    if (LockObject != null && LockObject.activeInHierarchy) LockObject.SetActive(false);
                    break;
                case EActiveStatus.Standby:
                    ActiveStatus = state;
                    if (StandbyObject != null && !StandbyObject.activeInHierarchy) StandbyObject.SetActive(true);
                    if (LockObject != null && LockObject.activeInHierarchy) LockObject.SetActive(false);
                    break;
                case EActiveStatus.Lock:
                    ActiveStatus = state;
                    if (StandbyObject != null && StandbyObject.activeInHierarchy) StandbyObject.SetActive(false);
                    if (LockObject != null && !LockObject.activeInHierarchy) LockObject.SetActive(true);
                    break;
            }
        }
    }
}