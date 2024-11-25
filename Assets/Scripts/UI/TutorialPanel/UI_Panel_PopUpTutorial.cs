using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_PopUpTutorial : MonoBehaviour
    {
        [SerializeField] private List<GameObject> uiPanelPopUpTutorialItem;
        
        public void RegisterReference()
        {
            foreach (GameObject tutorialPanelItem in uiPanelPopUpTutorialItem)
            {
                tutorialPanelItem.gameObject.SetActive(false);
            }
        }

        public void ActivatePanel(int index)
        {
            uiPanelPopUpTutorialItem[index].gameObject.SetActive(true);
        }
    }
}