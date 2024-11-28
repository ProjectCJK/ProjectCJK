using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;
using UnityEngine.UI;

namespace Units.Stages.Modules.UnlockModules.UI
{
    public class UnlockZoneView : BaseView<UnlockZoneViewModel>
    {
        public TextMeshProUGUI MaxGoldText;
        public Slider GoldSlider;

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
            MaxGoldText.text = $"<sprite=0> {viewModel.MaxGold}";

            if (viewModel.MaxGold != 0) GoldSlider.value = (float)viewModel.Gold / viewModel.MaxGold;
        }
    }
}