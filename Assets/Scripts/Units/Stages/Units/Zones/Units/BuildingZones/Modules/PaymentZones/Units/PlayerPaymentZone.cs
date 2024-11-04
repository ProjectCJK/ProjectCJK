using Units.Stages.Units.Zones.Units.BuildingZones.Modules.PaymentZones.Abstract;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Modules.PaymentZones.Units
{
    public interface IPlayerPaymentZone : IPaymentZone
    {
        
    }
    
    public class PlayerPaymentZone : PaymentZone, IPlayerPaymentZone
    {
        
    }
}