using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanels
{
    public class UI_Button_CustomerWave : MonoBehaviour
    {
        private Action onTimer;
        private const float existedTimeMaximum = 10f;
        private float existedTime;
        
        private int minutes;
        private int remainingSeconds;

        [SerializeField] private Image existedImage;
        [SerializeField] private TextMeshProUGUI existedTimeText;

        public void RegisterReference()
        {
            GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.UI_Panel_Main.UI_Panel_PopUp_Reward.Activate(EPopUpPanelType.CustomerRush, 0,
                () =>
                {
                    VolatileDataManager.Instance.CustomerTrigger = true;
                    Inactivate();
                }, null));
        }

        private void OnEnable()
        {
            existedTime = existedTimeMaximum;
        }

        private void Update()
        {
            existedTime -= Time.deltaTime;
            existedImage.fillAmount = existedTime / existedTimeMaximum;

            // 시간이 0보다 작아지면 0으로 설정
            if (existedTime < 0)
            {
                existedTime = 0;
                gameObject.SetActive(false);
                
                onTimer?.Invoke();
                onTimer = null;
            }

            ConvertFloatToTime(existedTime);
            
            // MM:SS 형식으로 텍스트 설정
            existedTimeText.text = $"{minutes:D2}:{remainingSeconds:D2}";
        }

        public void Activate(Action action)
        {
            onTimer = action;
            gameObject.SetActive(true);
        }

        private void Inactivate()
        {
            gameObject.SetActive(false);
        }
        
        private void ConvertFloatToTime(float seconds)
        {
            minutes = Mathf.FloorToInt(seconds / 60);
            remainingSeconds = Mathf.FloorToInt(seconds % 60);

            Debug.Log($"시간: {minutes}분 {remainingSeconds}초");
        }
    }
}