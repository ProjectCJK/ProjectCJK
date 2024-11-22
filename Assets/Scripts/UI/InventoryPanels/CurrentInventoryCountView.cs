using System.ComponentModel;
using System.Collections;
using Modules.DesignPatterns.MVVMs;
using TMPro;
using UnityEngine;

namespace UI.InventoryPanels
{
    public class CurrentInventoryCountView : BaseView<CurrentInventoryCountViewModel>
    {
        [SerializeField] private TextMeshProUGUI _text;

        private Coroutine _temporaryActivationCoroutine;

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
            if (viewModel.MaxInventorySize == 0) return;

            if (viewModel.CurrentInventorySize >= viewModel.MaxInventorySize)
            {
                _text.text = "<color=red><sprite=35> MAX</color>";
                
                if (!gameObject.activeSelf) gameObject.SetActive(true);
            }
            else if (viewModel.CurrentInventorySize == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                _text.text = $"<sprite=35> {viewModel.CurrentInventorySize.ToString()}/{viewModel.MaxInventorySize.ToString()}";
                
                if (_temporaryActivationCoroutine != null) StopCoroutine(_temporaryActivationCoroutine);
                
                if (!gameObject.activeSelf) gameObject.SetActive(true);
                if (gameObject.activeSelf) _temporaryActivationCoroutine = StartCoroutine(TemporaryActivationRoutine());
            }
        }

        private IEnumerator TemporaryActivationRoutine()
        {
            yield return new WaitForSeconds(1f);

            if (viewModel.CurrentInventorySize < viewModel.MaxInventorySize)
            {
                gameObject.SetActive(false);
            }
        }
    }
}