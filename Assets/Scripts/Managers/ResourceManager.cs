using Modules.DesignPatterns.Singletons;

namespace Managers
{
    public interface IResourceManager
    {
        
    }
    
    public class ResourceManager : Singleton<ResourceManager>, IResourceManager
    {
        
    }
}