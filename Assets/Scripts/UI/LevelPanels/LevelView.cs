using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;
using Units.Stages.UI.Level;
using UnityEngine.UI;

namespace UI.Level
{
    public class LevelView : BaseView<LevelViewModel>
    {
        public TextMeshProUGUI CurrentLevelText;
        public TextMeshProUGUI CurrentExpText;
        public Slider CurrentExpSlider;
        
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
            CurrentLevelText.text = $"{viewModel.CurrentLevel.ToString()}";
            CurrentExpText.text = $"{viewModel.CurrentExp.ToString()} / {viewModel.MaxExp.ToString()}";

            if (viewModel.MaxExp != 0)
            {
                CurrentExpSlider.value = (float) viewModel.CurrentExp / viewModel.MaxExp;   
            }
        }
    }
}