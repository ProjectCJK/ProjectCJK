using Units.Stages.Enums;
using Units.Stages.Modules.UnlockModules.Enums;

namespace Units.Stages.Modules.UnlockModules.Interfaces
{
    public interface IUnlockZoneProperty
    {
        public EUnlockZoneType UnlockZoneType { get; }
        public EActiveStatus ActiveStatus { get; }
        public int RequiredGoldForUnlock { get; }
        public int CurrentGoldForUnlock { get; }
    }
}