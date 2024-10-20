using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;
using UnityEngine.UI;

namespace Units.Stages.Buildings.UI
{
    public class BlenderProductView : BaseView<BlenderProductViewModel>
    {
        public TextMeshProUGUI remainedMaterialCountText;
        public Slider elapsedTimeSlider;

        protected override void BindUIElements()
        {
            // 뷰모델의 초기 값을 바인딩
            UpdateUI();
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            remainedMaterialCountText.text = viewModel.RemainedMaterialCount.ToString();
            
            if (viewModel.ProductLeadTime != 0) elapsedTimeSlider.value = viewModel.ElapsedTime / viewModel.ProductLeadTime;
        }
    }
}