using Modules.DesignPatterns.Singletons;

namespace Managers
{
    public class CostumeManager : SingletonMono<CostumeManager>
    {
        private string[,] _gameData;
        private QuestData _questData;
        
        public void RegisterReference()
        {
            // _gameData = DataManager.Instance.;
        }
    }
}