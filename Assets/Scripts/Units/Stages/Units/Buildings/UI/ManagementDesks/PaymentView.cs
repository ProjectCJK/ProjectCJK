using System;
using Managers;
using TMPro;
using Units.Stages.Modules;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Units.Stages.Units.Buildings.UI.ManagementDesks
{
    public class PaymentView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private Slider paymentSlider;

        private string _guestItemName;
        private int _guestItemCount;
        private float _maximumDelay;
        private Transform _targetTransform;
        private Vector3 _newPosition;
        
        public void Initialize(Tuple<string, int> guestItem, float playerPaymentDelay, Transform target)
        {
            _guestItemName = guestItem.Item1;
            _guestItemCount = guestItem.Item2;
            _maximumDelay = playerPaymentDelay;
            _targetTransform = target;
            
            (EItemType? itemType, EMaterialType? materialType) = ParserModule.ParseStringToEnum<EItemType, EMaterialType>(_guestItemName);

            var spriteIndex = 0;
            
            if (itemType != null && materialType != null)
            {
                if (itemType == EItemType.ProductA && materialType == EMaterialType.A)
                {
                    spriteIndex = 8;
                }
                else if (itemType == EItemType.ProductA && materialType == EMaterialType.B)
                {
                    spriteIndex = 9;
                }
                else if (itemType == EItemType.ProductA && materialType == EMaterialType.C)
                {
                    spriteIndex = 10;
                }
                else if (itemType == EItemType.ProductB && materialType == EMaterialType.A)
                {
                    spriteIndex = 11;
                }
            }

            itemCount.text = $"<sprite={spriteIndex}> {_guestItemCount}";
            
            UpdatePosition();
            
            gameObject.SetActive(true);
        }

        private void Update()
        {
            UpdatePosition();
        }

        public void UpdateUI(float paymentElapsedTime)
        {
            if (_maximumDelay != 0)
            {
                paymentSlider.value = paymentElapsedTime / _maximumDelay;
            }
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            
            itemCount.text = string.Empty;
            paymentSlider.value = 0;
            _targetTransform = null;
        }

        private void UpdatePosition()
        {
            if (_targetTransform != null)
            {
                // 타겟의 위치를 기반으로 새로운 위치 계산
                _newPosition.x = _targetTransform.position.x;
                _newPosition.y = _targetTransform.position.y + 1.5f;
                _newPosition.z = _targetTransform.position.z;

                // 위치 업데이트
                transform.position = _newPosition;   
            }
        }
    }
}