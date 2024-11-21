using System;
using Interfaces;
using Modules.DesignPatterns.Singletons;
using UI;
using UI.CurrencyPanel;
using Units.Stages.UI;
using Units.Stages.Units.Items.Enums;

namespace Managers
{
    public class CurrencyManager : Singleton<CurrencyManager>, IRegisterReference<CurrencyView>, IInitializable
    {
        private CurrencyView _currencyView;
        
        private CurrencyModel _currencyModel;
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

        public void RegisterReference(CurrencyView currencyView)
        {
            _currencyView = currencyView;

            _currencyModel = new CurrencyModel();
            _currencyViewModel = new CurrencyViewModel(_currencyModel);
            _currencyView.BindViewModel(_currencyViewModel);
        }
        
        public void Initialize()
        {
            _gold = GameManager.Instance.ES3Saver.Gold;
            _diamond = GameManager.Instance.ES3Saver.Diamond;
            _redGem = GameManager.Instance.ES3Saver.RedGem;

            UpdateViewModel();
        }

        public void AddCurrency(ECurrencyType currencyType, int value)
        {
            switch (currencyType)
            {
                case ECurrencyType.Diamond:
                    AddDiamond(value);
                    GameManager.Instance.ES3Saver.Diamond = Diamond;
                    break;
                case ECurrencyType.RedGem:
                    AddRedGem(value);
                    GameManager.Instance.ES3Saver.RedGem = RedGem;
                    break;
                case ECurrencyType.Gold:
                    AddGold(value);
                    GameManager.Instance.ES3Saver.Gold = Gold;
                    break;
            }
        }

        public void RemoveCurrency(ECurrencyType currencyType, int value)
        {
            switch (currencyType)
            {
                case ECurrencyType.Diamond:
                    RemoveDiamond(value);
                    ES3.Save($"{EES3Key.Diamond}", Diamond, ES3.settings);
                    break;
                case ECurrencyType.RedGem:
                    RemoveRedGem(value);
                    ES3.Save($"{EES3Key.RedGem}", RedGem, ES3.settings);
                    break;
                case ECurrencyType.Gold:
                    RemoveGold(value);
                    ES3.Save($"{EES3Key.Gold}", Gold, ES3.settings);
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