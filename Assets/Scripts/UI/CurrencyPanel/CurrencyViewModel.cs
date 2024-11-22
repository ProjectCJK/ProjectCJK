using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using Units.Stages.UI;

namespace UI.CurrencyPanel
{
    public class CurrencyViewModel : BaseViewModel
    {
        public int DiamondCount => _currencyModel.DiamondCount;
        public int RedGemCount => _currencyModel.RedGemCount;
        public int GoldCount => _currencyModel.GoldCount;
        
        private readonly CurrencyModel _currencyModel;

        public CurrencyViewModel(CurrencyModel currencyModel)
        {
            _currencyModel = currencyModel;
            _currencyModel.PropertyChanged += OnModelPropertyChanged;
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        
        public void UpdateValues(int diamondCount, int redGemCount, int goldCount)
        {
            _currencyModel.SetValues(diamondCount, redGemCount, goldCount);
        }
    }
}