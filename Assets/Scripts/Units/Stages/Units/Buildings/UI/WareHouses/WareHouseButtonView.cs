using System;
using TMPro;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Units.Stages.Units.Buildings.UI.WareHouses
{
    public class WareHouseButtonView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _itemCount;

        public Button _button;

        private bool isInitialized;

        public void InitializeButton(Sprite buttonDataSprite, int buttonDataItemCount, bool buttonDataIsInteractable,
            EMaterialType buttonDataMaterialType, Action<EMaterialType> buttonDataOnClickButton)
        {
            if (!isInitialized)
            {
                if (_button == null) _button = GetComponent<Button>();

                // 버튼 UI 초기 설정
                _image.sprite = buttonDataSprite;
                _itemCount.text = buttonDataItemCount > 0 ? $"x{buttonDataItemCount}" : string.Empty;
                _button.interactable = buttonDataIsInteractable;

                // 리스너를 처음 한 번만 추가
                _button.onClick.AddListener(() => buttonDataOnClickButton?.Invoke(buttonDataMaterialType));

                isInitialized = true;
            }
            else
            {
                // UI 업데이트만 수행
                _image.sprite = buttonDataSprite;
                _itemCount.text = buttonDataItemCount > 0 ? $"x{buttonDataItemCount}" : string.Empty;
                _button.interactable = buttonDataIsInteractable;
            }
        }
    }
}