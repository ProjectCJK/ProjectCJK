using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_Tutorial : MonoBehaviour
    {
        public event Action OnScriptsEnded;
        
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
                OnScriptsEnded?.Invoke();
            }
        }

        private void UpdateText()
        {
            _text.text = _scripts[_currentScriptIndex];

            _currentScriptIndex++;
            
            _isEnded = _currentScriptIndex == _scripts.Count;
        }
    }
}