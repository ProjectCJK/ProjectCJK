using System;
using Interfaces;
using Modules.DesignPatterns.Singletons;
using UI;
using UI.CurrencyPanel;
using Units.Stages.UI;
using Units.Stages.Units.Items.Enums;

namespace Managers
{
    public class CurrencyManager : Singleton<CurrencyManager>, IRegisterReference<UI_Panel_Currency>, IInitializable
    {
        private CurrencyModel _currencyModel;
        private UI_Panel_Currency _uiPanelCurrency;
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

        public void RegisterReference(UI_Panel_Currency uiPanelCurrency)
        {
            _uiPanelCurrency = uiPanelCurrency;

            _currencyModel = new CurrencyModel();
            _currencyViewModel = new CurrencyViewModel(_currencyModel);
            _uiPanelCurrency.BindViewModel(_currencyViewModel);
        }
        
        public void Initialize()
        {
            _gold = 100000;
            
            _currencyViewModel.UpdateValues(Diamond, RedGem, Gold);
        }

        public void AddCurrency(ECurrencyType currencyType, int value)
        {
            switch (currencyType)
            {
                case ECurrencyType.Money:
                    AddGold(value);
                    break;
                case ECurrencyType.Diamond:
                    AddDiamond(value);
                    break;
                case ECurrencyType.RedGem:
                    AddRedGem(value);
                    break;
                case ECurrencyType.Gold:
                    AddGold(value);
                    break;
            }
        }

        public void RemoveCurrency(ECurrencyType currencyType, int value)
        {
            switch (currencyType)
            {
                case ECurrencyType.Money:
                    RemoveGold(value);
                    break;
                case ECurrencyType.Diamond:
                    RemoveDiamond(value);
                    break;
                case ECurrencyType.RedGem:
                    RemoveRedGem(value);
                    break;
                case ECurrencyType.Gold:
                    RemoveGold(value);
                    break;
            }
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
        
        public void RemoveRedGem(int value)
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