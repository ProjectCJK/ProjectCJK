using Managers;
using Units.Stages.Enums;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;

namespace Units.Stages.Units.Buildings.Abstract
{
    public interface IUnlockableBuildingZoneProperty : IBuildingZone, IUnlockZoneProperty
    {
    }

    public abstract class UnlockableBuildingZone : BuildingZone, IUnlockableBuildingZoneProperty
    {
        public abstract UnlockZoneModule UnlockZoneModule { get; protected set; }
        public abstract EUnlockZoneType UnlockZoneType { get; }
        public abstract EActiveStatus ActiveStatus { get; }
        public abstract int RequiredGoldForUnlock { get; }
        public abstract int CurrentGoldForUnlock { get; set; }

        public override void Initialize()
        {
            GameManager.Instance.ES3Saver.RequiredMoneyForBuildingActive.TryAdd(BuildingKey, 100);
            CurrentGoldForUnlock = GameManager.Instance.ES3Saver.RequiredMoneyForBuildingActive[BuildingKey];
            UnlockZoneModule.CurrentGoldForUnlock = CurrentGoldForUnlock;
        }
        
        protected void HandleOnMoneyReceived(int value)
        {
            CurrentGoldForUnlock += value;
            UnlockZoneModule.CurrentGoldForUnlock = CurrentGoldForUnlock;

            UnlockZoneModule.UpdateViewModel();

            if (CurrentGoldForUnlock >= RequiredGoldForUnlock) UnlockZoneModule.SetCurrentState(EActiveStatus.Active);
        }
    }
}