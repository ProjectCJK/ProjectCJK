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
        
        public override void SetCurrentState(EActiveStatus state)
        {
            switch (state)
            {
                case EActiveStatus.Active:
                    ActiveStatus = state;
                    _playerCollision.SetActive(true);
                    if(StandbyObject != null) if (StandbyObject.activeInHierarchy) StandbyObject.SetActive(false);
                    if(LockObject != null) if (LockObject.activeInHierarchy) LockObject.SetActive(false);
                    break;
                case EActiveStatus.Standby:
                    ActiveStatus = state;
                    if(StandbyObject != null) if (!StandbyObject.activeInHierarchy) StandbyObject.SetActive(true);
                    if(LockObject != null) if (LockObject.activeInHierarchy) LockObject.SetActive(false);
                    break;
                case EActiveStatus.Lock:
                    ActiveStatus = state;
                    if(StandbyObject != null) if (StandbyObject.activeInHierarchy) StandbyObject.SetActive(false);
                    if(LockObject != null) if (!LockObject.activeInHierarchy) LockObject.SetActive(true);
                    break;
            }
        }
    }
}