namespace Units.Stages.Modules.InventoryModules.Interfaces
{
    public interface IBuildingItemReceiver : IItemReceiver
    {
        public void RegisterItemReceiver(ICreatureItemReceiver inventoryProperty);
        public void UnRegisterItemReceiver(ICreatureItemReceiver inventoryProperty);
    }
}