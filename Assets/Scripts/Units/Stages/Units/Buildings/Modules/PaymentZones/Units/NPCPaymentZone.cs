using System.Collections.Generic;
using Units.Stages.Units.Buildings.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Modules.PaymentZones.Units
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