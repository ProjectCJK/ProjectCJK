using System.Collections.Generic;
using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;
using UnityEngine;
using UnityEngine.UI;

namespace Units.Stages.Units.Buildings.UI.WareHouses
{
    public class WareHouseView : BaseView<WareHouseViewModel>
    {
        [SerializeField] private Button button;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private List<Button> buttons = new();

        private bool isInitialized;

        private void OnDisable()
        {
            button.gameObject.SetActive(true);
            buttonContainer.gameObject.SetActive(false);
        }

        protected override void BindUIElements()
        {
            if (!isInitialized)
            {
                // 처음 한 번만 리스너 추가
                button.onClick.AddListener(() =>
                {
                    button.gameObject.SetActive(false);
                    buttonContainer.gameObject.SetActive(true);
                });

                isInitialized = true;
            }

            UpdateButtonUI();
        }

        private void UpdateButtonUI()
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                var wareHouseButtonView = buttons[i].GetComponent<WareHouseButtonView>();

                if (viewModel.Buttons.Count > i)
                {
                    ButtonData buttonData = viewModel.Buttons[i];
                    wareHouseButtonView.InitializeButton(buttonData.Sprite, buttonData.ItemCount,
                        buttonData.IsInteractable, buttonData.MaterialType, buttonData.OnClickButton);
                }
                else
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Buttons)) UpdateButtonUI();
        }
    }
}