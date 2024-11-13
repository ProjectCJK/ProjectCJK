using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;

namespace Units.Stages.Units.Buildings.UI.Stands
{
    public class StandView : BaseView<StandViewModel>
    {
        public TextMeshPro remainedProductCountText;

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