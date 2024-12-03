using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_Tutorial_Initial : MonoBehaviour
    {
        public event Action OnClickExitButton;
        
        [SerializeField] private List<string> _scripts;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _button;

        private int _currentScriptIndex;
        private bool _isEnded;
        
        public void RegisterReference()
        {
            _button.onClick.AddListener(OnClickPanel);
        }
        
        public void Initialize()
        {
            _isEnded = false;
            _currentScriptIndex = 0;

            UpdateText();
        }

        private void OnClickPanel()
        {
            if (!_isEnded)
            {
                UpdateText();
            }
            else
            {
                OnClickExitButton?.Invoke();
            }
        }

        private void UpdateText()
        {
            _text.text = _scripts[_currentScriptIndex];

            if (!GameManager.Instance.ES3Saver.first_tutorial_tap && _currentScriptIndex == 0)
            {
                GameManager.Instance.ES3Saver.first_tutorial_tap = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("first_camera_complete");
            }
            else if (!GameManager.Instance.ES3Saver.second_tutorial_tap && _currentScriptIndex == 1)
            {
                GameManager.Instance.ES3Saver.second_tutorial_tap = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("second_tutorial_tap");
            }
            else if (!GameManager.Instance.ES3Saver.third_tutorial_tap && _currentScriptIndex == 2)
            {
                GameManager.Instance.ES3Saver.third_tutorial_tap = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("third_tutorial_tap");
            }

            _currentScriptIndex++;
            
            _isEnded = _currentScriptIndex == _scripts.Count;
        }
    }
}