using System;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;
using IPoolable = Modules.DesignPatterns.ObjectPools.IPoolable;

namespace UI.QuestPanels
{
    public class UI_Panel_QuestClearThumbnail_PopUp_Item : MonoBehaviour, IPoolable
    {
        private event Action OnReturn;

        [SerializeField] private Image targetQuestImage;
        [SerializeField] private TextMeshProUGUI targetQuestDescription;
        [SerializeField] private Color clearTextColor;

        private Animator _animator;
        private string _htmlColor;

        public void RegisterReference(Action action)
        {
            _animator = GetComponent<Animator>();
            _htmlColor = ColorUtility.ToHtmlStringRGBA(clearTextColor);
            
            OnReturn += action;
        }

        public void UpdateClearThumbnailQuestPopUp(UIQuestInfoItem questInfoItem)
        {
            targetQuestImage.sprite = questInfoItem.QuestIconImage;
            targetQuestDescription.text = $"<color=#{_htmlColor}>{questInfoItem.QuestDescriptionText}</color> is cleared!";

            gameObject.SetActive(true);
        }

        private void Update()
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            // 애니메이션 종료 체크
            if (stateInfo.normalizedTime >= 1f && stateInfo.IsTag("PopUp"))
            {
                OnReturn?.Invoke(); // 이벤트 호출
            }
        }

        public void Create()
        {
            gameObject.SetActive(false);
        }

        public void GetFromPool()
        {
            gameObject.SetActive(false);
        }

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
    }
}