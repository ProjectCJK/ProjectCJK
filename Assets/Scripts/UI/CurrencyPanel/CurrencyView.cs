using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;
using Units.Stages.UI;

namespace UI.CurrencyPanel
{
    public class CurrencyView : BaseView<CurrencyViewModel>
    {
        public TextMeshProUGUI DiamondText;
        public TextMeshProUGUI RedGemText;
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
            DiamondText.text = $"<sprite=2> {viewModel.DiamondCount.ToString()}";
            RedGemText.text = $"<sprite=4> {viewModel.RedGemCount.ToString()}";
            GoldText.text = $"<sprite=0> {viewModel.GoldCount.ToString()}";
        }
    }
}