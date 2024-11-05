using Units.Stages.Enums;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Abstract;
using UnityEngine;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Units
{
    public interface IDeliveryLodging
    {
        
    }
    
    public class DeliveryLodging : UnlockableBuildingZone, IDeliveryLodging
    {
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Transform TradeZoneNpcTransform { get; }
        
        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public override UnlockZoneModule UnlockZoneModule { get; protected set; }
        public override EUnlockZoneType UnlockZoneType { get; }
        public override EActiveStatus ActiveStatus { get; }
        public override int RequiredGoldForUnlock { get; }
        public override int CurrentGoldForUnlock { get; set; }
    }
}