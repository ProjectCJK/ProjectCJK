using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.PaymentZones.Abstract;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Modules.PaymentZones.Units
{
    public interface INPCPaymentZone : IPaymentZone
    {
        public bool CheckAccessorNPC(ENPCType npcType);
    }
    
    public class NPCPaymentZone : PaymentZone, INPCPaymentZone
    {
        private static ENPCType _inputAccessNPCType => ENPCType.Guest;
        
        public bool CheckAccessorNPC(ENPCType npcType)
        {
            return _inputAccessNPCType == npcType;
        }
    }
}