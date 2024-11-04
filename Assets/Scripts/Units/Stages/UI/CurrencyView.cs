using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;

namespace Units.Stages.UI
{
    public class CurrencyView : BaseView<CurrencyViewModel>
    {
        public TextMeshProUGUI GoldText;
        
        protected override void BindUIElements()
        {
            UpdateUI();
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            GoldText.text = viewModel.GoldCount.ToString();
        }
    }
}