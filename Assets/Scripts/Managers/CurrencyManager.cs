using Interfaces;
using Modules.DesignPatterns.Singletons;
using Units.Stages.UI;

namespace Managers
{
    public class CurrencyManager : Singleton<CurrencyManager>, IRegisterReference<UI_Currency>, IInitializable
    {
        private CurrencyModel _currencyModel;
        private UI_Currency _uiCurrency;
        private CurrencyViewModel _currencyViewModel;

        private int _redGem;
        public int RedGem
        {
            get => _redGem;
            private set
            {
                _redGem = value;
                UpdateViewModel();
            }
        }
        
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

        private int _diamond;
        public int Diamond
        {
            get => _diamond;
            private set
            {
                _diamond = value;
                UpdateViewModel();
            }
        }

        public void RegisterReference(UI_Currency uiCurrency)
        {
            _uiCurrency = uiCurrency;

            _currencyModel = new CurrencyModel();
            _currencyViewModel = new CurrencyViewModel(_currencyModel);
            _uiCurrency.BindViewModel(_currencyViewModel);
        }
        
        public void Initialize()
        {
            _gold = 100000;
            
            _currencyViewModel.UpdateValues(Diamond, RedGem, Gold);
        }

        public void AddGold(int value)
        {
            Gold += value;
        }

        public void RemoveGold(int value)
        {
            Gold -= value;
        }
        
        public void AddRedGem(int value)
        {
            RedGem += value;
        }
        
        public void RemoveRedGed(int value)
        {
            RedGem -= value;
        }
        
        public void AddDiamond(int value)
        {
            Diamond += value;
        }
                
        public void RemoveDiamond(int value)
        {
            Diamond -= value;
        }

        private void UpdateViewModel()
        {
            _currencyViewModel.UpdateValues(Diamond, RedGem, Gold);
        }
    }
}