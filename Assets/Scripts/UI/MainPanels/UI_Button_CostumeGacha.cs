using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanels
{
    public class UI_Button_CostumeGacha : MonoBehaviour
    {
        public event Action OnClickButton;
        
        public void RegisterReference()
        {
            var button = GetComponent<Button>();
            
            button.onClick.AddListener(
                () => UIManager.Instance.UI_Panel_Main.UI_Panel_PopUp_Reward.Activate(
                    EPopUpPanelType.CostumeGacha,
                    10, 
                    () => OnClickButton?.Invoke(),
                    () => OnClickButton?.Invoke()));
        }
    }
}