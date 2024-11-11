using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.UI
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
            OnPropertyChanged(e.PropertyName); // 변경된 프로퍼티를 UI에 알림
        }

        // 모델에 새로운 값을 설정하는 메서드
        public void UpdateValues(int diamondCount, int redGemCount, int goldCount)
        {
            _currencyModel.SetValues(diamondCount, redGemCount, goldCount);
        }
    }
}