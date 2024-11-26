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
            if (!transform.parent.gameObject.activeInHierarchy) return;
            
            if (viewModel.MaxInventorySize == 0) return;

            if (viewModel.CurrentInventorySize >= viewModel.MaxInventorySize)
            {
                _text.text = "<color=red><sprite=35> MAX</color>";
                SetGameObjectActive(true);
            }
            else if (viewModel.CurrentInventorySize == 0)
            {
                SetGameObjectActive(false);
            }
            else
            {
                _text.text = $"<sprite=35> {viewModel.CurrentInventorySize}/{viewModel.MaxInventorySize}";

                if (_temporaryActivationCoroutine != null)
                    StopCoroutine(_temporaryActivationCoroutine);

                SetGameObjectActive(true);
            }
        }

        private void SetGameObjectActive(bool isActive)
        {
            if (gameObject.activeSelf != isActive)
                gameObject.SetActive(isActive);
        }
    }
}