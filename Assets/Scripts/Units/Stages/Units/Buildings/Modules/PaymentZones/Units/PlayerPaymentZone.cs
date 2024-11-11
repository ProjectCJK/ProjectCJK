using Units.Stages.Units.Buildings.Modules.PaymentZones.Abstract;

namespace Units.Stages.Units.Buildings.Modules.PaymentZones.Units
{
    public interface IPlayerPaymentZone : IPaymentZone
    {
    }

    public class PlayerPaymentZone : PaymentZone, IPlayerPaymentZone
    {
    }
}