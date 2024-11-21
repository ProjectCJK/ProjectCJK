using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Level
{
    public class UI_Panel_LevelUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        
        [Space(20), SerializeField] private Image reward1Icon;
        [SerializeField] private TextMeshProUGUI reward1Count;
        [SerializeField] private Image reward2Icon;
        [SerializeField] private TextMeshProUGUI reward2Count;
        
        [Space(20), SerializeField] private Button skipButton;
        [SerializeField] private Button adButton;
    }
}