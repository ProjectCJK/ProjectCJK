using Interfaces;
using Units.Modules.FactoryModules.Units;

namespace Units.Stages.Controllers
{
    public interface IGuestController : IInitializable
    {
        public IGuestFactory GuestFactory { get; }
        
    }
    
    public class GuestController : IGuestController
    {
        public IGuestFactory GuestFactory { get; }
        
        public GuestController()
        {
            GuestFactory = new GuestFactory();
            GuestFactory.CreateGuest();
        }

        public void Initialize()
        {
            // ObjectPoolManager.Instance.GetObject(GuestFactory.PoolKey, null);
        }
    }
}