using Modules.DesignPatterns.Singletons;
using Unity.IO.LowLevel.Unsafe;

namespace Managers
{
    public interface ICurrencyManager
    {
        
    }
    
    public class CurrencyManager : Singleton<CurrencyManager>, ICurrencyManager
    {
        public float Gold { get; set; } = 0;
        public float Diamond { get; set; } = 0;
        public float Cookie { get; set; } = 0;
    }
}