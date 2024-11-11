namespace Units.Stages.Modules.PaymentModule.Abstract
{
    public interface IBuildingPaymentModule : IPaymentModule
    {
    }

    public abstract class BuildingPaymentModule : PaymentModule, IBuildingPaymentModule
    {
    }
}