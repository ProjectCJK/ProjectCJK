using Modules.DesignPatterns.Singletons;
using UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : SingletonMono<UIManager>
    {
        [Header("=== 건물 UI ===")]
        [SerializeField] private UI_BuildingEnhancement uiBuildingEnhancement;
    }
}