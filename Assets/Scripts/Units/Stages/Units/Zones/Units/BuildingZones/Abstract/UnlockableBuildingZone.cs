using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Abstract
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
    }
}