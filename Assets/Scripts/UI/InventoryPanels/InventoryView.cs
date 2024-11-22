using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using TMPro;
using UnityEngine;

namespace UI.InventoryPanels
{
    public class InventoryView : BaseView<InventoryViewModel>
    {
        public TextMeshProUGUI TomatoText;
        public TextMeshProUGUI CucumberText;
        public TextMeshProUGUI BeanText;
        public TextMeshProUGUI TomatoCanText;
        public TextMeshProUGUI CucumberCanText;
        public TextMeshProUGUI BeanCanText;
        public TextMeshProUGUI TomatoStewCanText;
        
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
            UpdateText(TomatoText, viewModel.TomatoCount, "<sprite=5>");
            UpdateText(CucumberText, viewModel.CucumberCount, "<sprite=6>");
            UpdateText(BeanText, viewModel.BeanCount, "<sprite=7>");
            UpdateText(TomatoCanText, viewModel.TomatoCanCount, "<sprite=8>");
            UpdateText(CucumberCanText, viewModel.CucumberCanCount, "<sprite=9>");
            UpdateText(BeanCanText, viewModel.BeanCanCount, "<sprite=10>");
            UpdateText(TomatoStewCanText, viewModel.TomatoStewCanCount, "<sprite=11>");
        }

        private void UpdateText(TextMeshProUGUI text, int count, string spriteTag)
        {
            if (count != 0)
            {
                if (!text.gameObject.activeSelf) text.gameObject.SetActive(true);
                text.text = $"{spriteTag} {count.ToString()}";
            }
            else
            {
                if (text.gameObject.activeSelf) text.gameObject.SetActive(false);
            }
        }
    }
}