using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;

namespace Units.Stages.Buildings.UI.FoodStands
{
    public class FoodStandView : BaseView<FoodStandViewModel>
    {
        public TextMeshProUGUI remainedProductCountText;

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
            remainedProductCountText.text = viewModel.RemainedProductCount.ToString();
        }
    }
}