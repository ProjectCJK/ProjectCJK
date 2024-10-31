using Units.Stages.Units.Buildings.Modules.PaymentZones.Abstract;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Modules.PaymentZones.Units
{
    public interface IPlayerPaymentZone : IPaymentZone
    {
        
    }
    
    public class PlayerPaymentZone : PaymentZone, IPlayerPaymentZone
    {
        
    }
}