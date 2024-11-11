namespace Units.Stages.Modules.MovementModules.Abstract
{
    public interface IMovementModule
    {
    }

    public interface IMovementProperty
    {
        float MovementSpeed { get; set; }
    }

    public abstract class MovementModule : IMovementModule
    {
    }
}