using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public interface IUI_BuildingEnhancement : IInitializable
    {
        
    }
    
    public class UI_BuildingEnhancement : MonoBehaviour, IUI_BuildingEnhancement
    {
        [Header("=== 패널 타이틀 ===")]
        [SerializeField] private TextMeshProUGUI Text_popUpTitle;
        [SerializeField] private TextMeshProUGUI Text_buildingLevel;
        
        public void Initialize()
        {
            Debug.Log("UI_BuildingEnhancement.Initialize");
        }
    }
}