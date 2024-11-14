using System;
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
                    if (TargetObject != null && !TargetObject.activeInHierarchy) TargetObject.SetActive(true);
                    if (StandbyObject != null && StandbyObject.activeInHierarchy) StandbyObject.SetActive(false);
                    if (LockObject != null && LockObject.activeInHierarchy) LockObject.SetActive(false);
                    OnChangeActiveStatus?.Invoke(TargetKey, EActiveStatus.Active);
                    break;
                case EActiveStatus.Standby:
                    ActiveStatus = state;
                    if (TargetObject != null && TargetObject.activeInHierarchy) TargetObject.SetActive(false);
                    if (StandbyObject != null && !StandbyObject.activeInHierarchy) StandbyObject.SetActive(true);
                    if (LockObject != null && LockObject.activeInHierarchy) LockObject.SetActive(false);
                    break;
                case EActiveStatus.Lock:
                    ActiveStatus = state;
                    if (TargetObject != null && TargetObject.activeInHierarchy) TargetObject.SetActive(false);
                    if (StandbyObject != null && StandbyObject.activeInHierarchy) StandbyObject.SetActive(false);
                    if (LockObject != null && !LockObject.activeInHierarchy) LockObject.SetActive(true);
                    break;
            }
        }
    }
}