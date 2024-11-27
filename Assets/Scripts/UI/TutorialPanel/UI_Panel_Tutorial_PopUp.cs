using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_Tutorial_PopUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _balloonText;
        [SerializeField] private Button closeButton;
        
        private Dictionary<int, string> textLists = new();
        
        public Action<int> OnClickExitButton;

        public void RegisterReference()
        {
            textLists.TryAdd(2, "It looks like there might be monsters over there. Let's go check it out.");
            textLists.TryAdd(3, "There was a monster after all! Let's hunt the monster with this torch!");
            textLists.TryAdd(4, "Now let's put the tomatoes in the canning machine to make canned tomatoes.");
            textLists.TryAdd(5, "Let's put the canned tomatoes on the food stand so that the survivors can buy them.");
            textLists.TryAdd(6,"Okay. Now let's go ring up the items.");
            textLists.TryAdd(7, "Let's upgrade the canning machine with the money we earned.");
            textLists.TryAdd(8, "We'll probably need some extra hands. Let's hire a cashier.");
            textLists.TryAdd(9, "Now that we can take a breather, let's get some better equipment!");
            textLists.TryAdd(10, "Build a lodge to hire a deliveryman who will carry your products to the food stand.");
            textLists.TryAdd(11, "Build a warehouse to hire a hunter who will hunt monsters and stock up materials.");
            
            // if (GameManager.Instance.ES3Saver.PopUpTutorialClear.Count == 0)
            // {
            //     for (var i = 0 ; i < uiPanelPopUpTutorialItem.Count ; i++)
            //     {
            //         GameManager.Instance.ES3Saver.PopUpTutorialClear.TryAdd(i, false);
            //     }
            // }
        }

        public void ActivatePanel(int index)
        {
            _balloonText.text = textLists[index];
            gameObject.SetActive(true);
            
            closeButton.onClick.RemoveAllListeners();;
            closeButton.onClick.AddListener(() =>
            {
                OnClickExitButton?.Invoke(index);
                gameObject.SetActive(false);
            });
        }
    }
}