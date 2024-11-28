using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanels
{
    public class UI_Panel_MainButtons : MonoBehaviour
    {
        public Button UI_Button_CostumePanel;
        public Button UI_Button_StageMap;
        public List<Button> UI_Button_LockButton;
        
        public UI_Button_CostumeGacha UI_Button_CostumeGacha;
        public UI_Button_SuperHunter UI_Button_SuperHunter;
        public UI_Button_CustomerWave UI_Button_CustomerWave;

        public void RegisterReference()
        {
            UI_Button_CostumeGacha.RegisterReference();
            UI_Button_SuperHunter.RegisterReference();
            UI_Button_CustomerWave.RegisterReference();
        }
    }
}