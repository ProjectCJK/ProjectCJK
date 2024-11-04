using Interfaces;
using Modules.DesignPatterns.Singletons;
using Units.Stages.UI;
using Unity.IO.LowLevel.Unsafe;

namespace Managers
{
    public class CurrencyManager : Singleton<CurrencyManager>, IRegisterReference<CurrencyView>, IInitializable
    {
        private int _gold;
        public int Gold 
        {
            get => _gold;
            private set
            {
                _gold = value;
                UpdateViewModel();
            }
        }
        
        public float Diamond { get; set; } = 0;
        public float Cookie { get; set; } = 0;

        private CurrencyModel _currencyModel;
        private CurrencyViewModel _currencyViewModel;
        private CurrencyView _currencyView;

        public void RegisterReference(CurrencyView currencyView)
        {
            _currencyView = currencyView;
            
            _currencyModel = new CurrencyModel();
            _currencyViewModel = new CurrencyViewModel(_currencyModel);
            _currencyView.BindViewModel(_currencyViewModel);
        }

        public void Initialize()
        {
            _currencyViewModel.UpdateValues(Gold);
        }
        
        public void AddGold(int value)
        {
            Gold += value;
        }

        public void RemoveGold(int value)
        {
            Gold -= value;
        }

        private void UpdateViewModel()
        {
            _currencyViewModel.UpdateValues(Gold);
        }
    }
}