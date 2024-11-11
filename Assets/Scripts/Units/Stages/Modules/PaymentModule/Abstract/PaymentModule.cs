namespace Units.Stages.Modules.PaymentModule.Abstract
{
    public interface IPaymentProperty
    {
        public float PaymentDelay { get; }
    }

    public interface IPaymentModule
    {
    }

    public abstract class PaymentModule : IPaymentModule
    {
    }
}