using System;
using Interfaces;
using Modules.DesignPatterns.Singletons;
using UI;
using UI.CurrencyPanel;
using Units.Stages.UI;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Managers
{
    public class CurrencyManager : Singleton<CurrencyManager>, IRegisterReference, IInitializable
    {
        private CurrencyView _currencyView;
        private CurrencyModel _currencyModel;
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

        public void RegisterReference()
        {
            _currencyView = UIManager.Instance.UI_Panel_Main.CurrencyView;

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
        
        private void UpdateCurrency(ECurrencyType currencyType, int value)
        {
            switch (currencyType)
            {
                case ECurrencyType.Diamond:
                    Diamond = Mathf.Max(0, Diamond + value);
                    GameManager.Instance.ES3Saver.Diamond = Diamond;
                    break;
                case ECurrencyType.RedGem:
                    RedGem = Mathf.Max(0, RedGem + value);
                    GameManager.Instance.ES3Saver.RedGem = RedGem;
                    break;
                case ECurrencyType.Gold:
                    Gold = Mathf.Max(0, Gold + value);
                    GameManager.Instance.ES3Saver.Gold = Gold;
                    break;
            }
        }

        public void AddCurrency(ECurrencyType currencyType, int value)
        {
            UpdateCurrency(currencyType, value);
        }

        public void RemoveCurrency(ECurrencyType currencyType, int value)
        {
            UpdateCurrency(currencyType, -value);
        }

        private void UpdateViewModel()
        {
            _currencyViewModel.UpdateValues(Diamond, RedGem, Gold);
        }
    }
}