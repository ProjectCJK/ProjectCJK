using Interfaces;

namespace Units.Stages.Controllers
{
    public interface IGuestController : IRegisterReference
    {
        
    }
    
    public class GuestController : IGuestController
    {
        public void RegisterReference()
        {
            throw new System.NotImplementedException();
        }
    }
}