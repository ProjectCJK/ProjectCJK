using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.UI.Level
{
    public class LevelModel : BaseModel
    {
        private int _currentLevel;
        private int _currentExp;
        private int _maxExp;

        public int CurrentLevel
        {
            get => _currentLevel;
            
            private set
            {
                if (SetField(ref _currentLevel, value)) OnPropertyChanged();
            }
        }
        
        public int CurrentExp
        {
            get => _currentExp;
            
            private set
            {
                if (SetField(ref _currentExp, value)) OnPropertyChanged();
            }
        }
        
        public int MaxExp
        {
            get => _maxExp;
            
            private set
            {
                if (SetField(ref _maxExp, value)) OnPropertyChanged();
            }
        }
        
        public void SetValues(int currentLevel, int currentExp, int maxExp)
        {
            CurrentLevel = currentLevel;
            CurrentExp = currentExp;
            MaxExp = maxExp;
        }
    }
}