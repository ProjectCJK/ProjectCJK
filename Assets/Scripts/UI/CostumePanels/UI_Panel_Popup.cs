using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumePanels
{
    public class UI_Panel_Popup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI costumeNameText;
        [SerializeField] private List<Image> costumeTypeImage;
        [SerializeField] private TextMeshProUGUI costumeTypeText;
        
        [Space(20), SerializeField] private TextMeshProUGUI costumeLevelText;
        [SerializeField] private Image costumeImageBackgroundImage;
        [SerializeField] private Image costumeIconImage;
        
        [Space(20), SerializeField] private TextMeshProUGUI costumeDescriptionText;
        [SerializeField] private TextMeshProUGUI costumeOption1DescriptionText;

        public void RegisterReference()
        {
            
        }
    }
}