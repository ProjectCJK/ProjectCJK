using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;
using UnityEngine.UI;

namespace Units.Stages.Units.Zones.Units.BuildingZones.UI.Kitchens
{
    public class KitchenView : BaseView<KitchenViewModel>
    {
        public TextMeshProUGUI remainedMaterialCountText;
        public Slider elapsedTimeSlider;

        // 초기 UI 바인딩
        protected override void BindUIElements()
        {
            UpdateUI(); // 뷰모델의 초기 값을 바인딩
        }

        // 뷰모델의 값이 변경될 때 UI를 업데이트
        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateUI(); // 값이 변경될 때마다 UI 갱신
        }

        // UI 업데이트
        private void UpdateUI()
        {
            remainedMaterialCountText.text = viewModel.RemainedMaterialCount.ToString();
            
            if (viewModel.ProductLeadTime != 0)
            {
                elapsedTimeSlider.value = viewModel.ElapsedTime / viewModel.ProductLeadTime;
            }
        }
    }
}