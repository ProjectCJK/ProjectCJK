using Modules.DesignPatterns.Singletons;

namespace Managers
{
    public interface IGameManager
    {
        
    }
    public class GameManager : SingletonMono<GameManager>, IGameManager
    {
        
    }
}