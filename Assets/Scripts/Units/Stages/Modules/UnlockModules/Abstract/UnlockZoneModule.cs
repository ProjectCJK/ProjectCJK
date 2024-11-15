using System;
using Interfaces;
using Units.Stages.Enums;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Modules.UnlockModules.UI;
using UnityEngine;

namespace Units.Stages.Modules.UnlockModules.Abstract
{
    public interface IUnlockZoneModule : IUnlockZoneProperty, IRegisterReference<string>
    {
        public void SetCurrentState(EActiveStatus state);
        public void UpdateViewModel();
    }

    public abstract class UnlockZoneModule : MonoBehaviour, IUnlockZoneModule
    {
        [SerializeField] private UnlockZoneView _unlockZoneView;

        public GameObject StandbyObject;
        public GameObject LockObject;

        private UnlockZoneModel _unlockZoneModel;
        private UnlockZoneViewModel _unlockZoneViewModel;
        public string TargetKey { get; private set; }

        public abstract EUnlockZoneType UnlockZoneType { get; }
        public EActiveStatus ActiveStatus { get; protected set; }
        public int CurrentGoldForUnlock { get; set; }
        public int RequiredGoldForUnlock { get; set; }

        public abstract void SetCurrentState(EActiveStatus state);

        public void RegisterReference(string targetKey)
        {
            TargetKey = targetKey;
            ActiveStatus = EActiveStatus.Lock;
            _unlockZoneModel = new UnlockZoneModel();
            _unlockZoneViewModel = new UnlockZoneViewModel(_unlockZoneModel);
            _unlockZoneView.BindViewModel(_unlockZoneViewModel);
        }

        public void UpdateViewModel()
        {
            _unlockZoneViewModel.UpdateValues(CurrentGoldForUnlock, RequiredGoldForUnlock);
        }

        public abstract event Action<string, EActiveStatus> OnChangeActiveStatus;
    }
}