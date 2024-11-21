using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.UI.Level
{
    public class LevelViewModel : BaseViewModel
    {
        public int CurrentLevel => _levelModel.CurrentLevel;
        public int CurrentExp => _levelModel.CurrentExp;
        public int MaxExp => _levelModel.MaxExp;
        
        private readonly LevelModel _levelModel;

        public LevelViewModel(LevelModel levelModel)
        {
            _levelModel = levelModel;
            _levelModel.PropertyChanged += OnModelPropertyChanged;
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName); 
        }
        
        public void UpdateValues(int currentLevel, int currentExp, int maxExp)
        {
            _levelModel.SetValues(currentLevel, currentExp, maxExp);
        }
    }
}