using System;
using UnityEngine;
using UnityEngine.UI;

namespace Units.Stages.Modules
{
    public class GameSpeedControlModule : MonoBehaviour
    {
        public Slider speedSlider; // 슬라이더를 연결할 변수

        private void Awake()
        {
            speedSlider = GetComponent<Slider>();
            speedSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            // 슬라이더 값에 따라 게임 속도 조정
            Time.timeScale = value * 2; // 0에서 1까지의 값을 0에서 10으로 변환
        }
    }
}