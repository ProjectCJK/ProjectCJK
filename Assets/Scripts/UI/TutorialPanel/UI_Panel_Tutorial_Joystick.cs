using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_Tutorial_Joystick : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        private readonly WaitForSeconds _delayTimer = new(1f);

        private void OnEnable()
        {
            if (_button.gameObject.activeSelf) _button.gameObject.SetActive(false);
            
            StartCoroutine(InactivateJoystickTutorialPanel());
        }

        private IEnumerator InactivateJoystickTutorialPanel()
        {
            yield return _delayTimer;
            
            _button.gameObject.SetActive(true);
        }
    }
}