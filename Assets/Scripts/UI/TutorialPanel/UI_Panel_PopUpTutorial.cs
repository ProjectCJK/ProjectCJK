using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_PopUpTutorial : MonoBehaviour
    {
        [SerializeField] private List<UI_Panel_PopUpTutorialItem> uiPanelPopUpTutorialItem;
        
        public void RegisterReference()
        {
            foreach (UI_Panel_PopUpTutorialItem t in uiPanelPopUpTutorialItem)
            {
                t.RegisterReference();
                t.gameObject.SetActive(false);
            }
        }

        public void ActivatePanel(int index)
        {
            uiPanelPopUpTutorialItem[index].gameObject.SetActive(true);
        }
    }
}