namespace Units.Stages.Modules.HealthModules.Abstract
{
    public interface IHealthProperty
    {
        public int MaxHealth { get; }
    }
    
    public interface IHealthModule
    {
    
    }
    
    public abstract class HealthModule : IHealthModule
    {
        
    }
}