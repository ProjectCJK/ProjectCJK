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

        public int _redGem;
        public int RedGem
        {
            get => _redGem;
            private set
            {
                _redGem = value;
                UpdateViewModel();
            }
        }
        
        public int _gold;
        public int Gold
        {
            get => _gold;
            private set
            {
                _gold = value;
                UpdateViewModel();
            }
        }

        public int _diamond;
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

            _gold = ES3.KeyExists($"{EES3Key.Gold}") ? ES3.Load<int>($"{EES3Key.Gold}") : 10000;
            _diamond = ES3.KeyExists($"{EES3Key.Diamond}") ? ES3.Load<int>($"{EES3Key.Diamond}") : 0;
            _redGem = ES3.KeyExists($"{EES3Key.RedGem}") ? ES3.Load<int>($"{EES3Key.RedGem}") : 0;
        }
        
        public void Initialize()
        {
            _currencyViewModel.UpdateValues(Diamond, RedGem, Gold);
        }

        public void AddCurrency(ECurrencyType currencyType, int value)
        {
            switch (currencyType)
            {
                case ECurrencyType.Diamond:
                    AddDiamond(value);
                    ES3.Save($"{EES3Key.Diamond}", Diamond);
                    break;
                case ECurrencyType.RedGem:
                    AddRedGem(value);
                    ES3.Save($"{EES3Key.RedGem}", RedGem);
                    break;
                case ECurrencyType.Gold:
                    AddGold(value);
                    ES3.Save($"{EES3Key.Gold}", Gold);
                    break;
            }
        }

        public void RemoveCurrency(ECurrencyType currencyType, int value)
        {
            switch (currencyType)
            {
                case ECurrencyType.Diamond:
                    RemoveDiamond(value);
                    ES3.Save($"{EES3Key.Diamond}", Diamond);
                    break;
                case ECurrencyType.RedGem:
                    RemoveRedGem(value);
                    ES3.Save($"{EES3Key.RedGem}", RedGem);
                    break;
                case ECurrencyType.Gold:
                    RemoveGold(value);
                    ES3.Save($"{EES3Key.Gold}", Gold);
                    break;
            }
        }

        private void AddGold(int value)
        {
            Gold += value;
        }

        private void RemoveGold(int value)
        {
            Gold -= value;
        }
        
        private void AddRedGem(int value)
        {
            RedGem += value;
        }
        
        private void RemoveRedGem(int value)
        {
            RedGem -= value;
        }
        
        private void AddDiamond(int value)
        {
            Diamond += value;
        }
                
        private void RemoveDiamond(int value)
        {
            Diamond -= value;
        }

        private void UpdateViewModel()
        {
            _currencyViewModel.UpdateValues(Diamond, RedGem, Gold);
        }
    }
}