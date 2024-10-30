using Units.Modules.InventoryModules.Abstract;

namespace Units.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract
{
    public interface ICreatureInventoryModule : IInventoryModule
    {
        
    }
    
    public abstract class CreatureInventoryModule : InventoryModule, ICreatureInventoryModule
    {
        
    }
}