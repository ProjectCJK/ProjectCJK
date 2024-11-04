using Units.Stages.Enums;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using UnityEngine;

namespace Units.Stages.Modules.UnlockModules.Units
{
    public class BuildingUnlockZoneModule : UnlockZoneModule
    {
        public override EUnlockZoneType UnlockZoneType => EUnlockZoneType.Building;
        
        public GameObject TargetObject;
        
        public override void SetCurrentState(EActiveStatus state)
        {
            switch (state)
            {
                case EActiveStatus.Active:
                    ActiveStatus = state;
                    if(TargetObject != null) if (!TargetObject.activeInHierarchy) TargetObject.SetActive(false);
                    if(StandbyObject != null) if (StandbyObject.activeInHierarchy) StandbyObject.SetActive(false);
                    if(LockObject != null) if (LockObject.activeInHierarchy) LockObject.SetActive(false);
                    break;
                case EActiveStatus.Standby:
                    ActiveStatus = state;
                    if(TargetObject != null) if (TargetObject.activeInHierarchy) TargetObject.SetActive(false);
                    if(StandbyObject != null) if (!StandbyObject.activeInHierarchy) StandbyObject.SetActive(true);
                    if(LockObject != null) if (LockObject.activeInHierarchy) LockObject.SetActive(false);
                    break;
                case EActiveStatus.Lock:
                    ActiveStatus = state;
                    if(TargetObject != null) if (TargetObject.activeInHierarchy) TargetObject.SetActive(false);
                    if(StandbyObject != null) if (StandbyObject.activeInHierarchy) StandbyObject.SetActive(false);
                    if(LockObject != null) if (!LockObject.activeInHierarchy) LockObject.SetActive(true);
                    break;
            }
        }
    }
}