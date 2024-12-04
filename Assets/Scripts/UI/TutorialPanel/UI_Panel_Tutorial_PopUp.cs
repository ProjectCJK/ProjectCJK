using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using Units.Stages.Managers;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_Tutorial_PopUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _balloonText;
        [SerializeField] private Button closeButton;
        
        private readonly Dictionary<int, string> textLists = new();
        
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
            textLists.TryAdd(50, "Build a lodge to hire a deliveryman who will carry your products to the food stand.");
            textLists.TryAdd(56, "Build a warehouse to hire a hunter who will hunt monsters and stock up materials.");
            
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
            if (!GameManager.Instance.ES3Saver.first_tutorial_popup_tap)
            {
                GameManager.Instance.ES3Saver.first_tutorial_popup_tap = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("first_tutorial_popup_tap");
            }
            
            if (index == 4)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_popup_product_tap");
            }
            else if (index == 6)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_popup_sell_tap");
            }
            else if (index == 9)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_popup_costume_tap");
            }
            else if (index == 50)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_popup_delivery_deliveryMan_tap");
            }
            else if (index == 56)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_popup_warehouse_hunter_tap");
            }

            if (index >= 7)
            {
                GameManager.Instance.ES3Saver.UpgradeZoneTrigger = true;
                
                if (MainSceneManager.Instance.StageController.BuildingController.Buildings.ContainsKey($"{EBuildingType.KitchenA}_{EMaterialType.A}"))
                {
                    if (MainSceneManager.Instance.StageController.BuildingController.Buildings[$"{EBuildingType.KitchenA}_{EMaterialType.A}"] is Kitchen kitchen)
                    {
                        kitchen._kitchenDefaultSetting.UpgradeZone_Player.gameObject.SetActive(true);
                    }
                }
                
                if (MainSceneManager.Instance.StageController.BuildingController.Buildings.ContainsKey($"{EBuildingType.KitchenA}_{EMaterialType.B}"))
                {
                    if (MainSceneManager.Instance.StageController.BuildingController.Buildings[$"{EBuildingType.KitchenA}_{EMaterialType.B}"] is Kitchen kitchen)
                    {
                        kitchen._kitchenDefaultSetting.UpgradeZone_Player.gameObject.SetActive(true);
                    }
                }
                
                if (MainSceneManager.Instance.StageController.BuildingController.Buildings.ContainsKey($"{EBuildingType.KitchenA}_{EMaterialType.C}"))
                {
                    if (MainSceneManager.Instance.StageController.BuildingController.Buildings[$"{EBuildingType.KitchenA}_{EMaterialType.C}"] is Kitchen kitchen)
                    {
                        kitchen._kitchenDefaultSetting.UpgradeZone_Player.gameObject.SetActive(true);
                    }
                }
                
                if (MainSceneManager.Instance.StageController.BuildingController.Buildings.ContainsKey($"{EBuildingType.KitchenB}_{EMaterialType.A}"))
                {
                    if (MainSceneManager.Instance.StageController.BuildingController.Buildings[$"{EBuildingType.KitchenB}_{EMaterialType.A}"] is Kitchen kitchen)
                    {
                        kitchen._kitchenDefaultSetting.UpgradeZone_Player.gameObject.SetActive(true);
                    }
                }
                
                if (MainSceneManager.Instance.StageController.BuildingController.Buildings.ContainsKey($"{EBuildingType.ManagementDesk}"))
                {
                    if (MainSceneManager.Instance.StageController.BuildingController.Buildings[$"{EBuildingType.ManagementDesk}"] is ManagementDesk managementDesk)
                    {
                        managementDesk._managementDeskDefaultSetting.UpgradeZone_Player.gameObject.SetActive(true);
                    }
                }
            }
                
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