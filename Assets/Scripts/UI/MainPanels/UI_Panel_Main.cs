using UI.BuildingEnhancementPanel;
using UI.CostumeGachaPanels;
using UI.CostumePanels;
using UI.CurrencyPanel;
using UI.InventoryPanels;
using UI.Level;
using UI.LevelPanels;
using UI.QuestPanels;
using UI.StageMapPanel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanels
{
    public class UI_Panel_Main : MonoBehaviour
    {
        public LevelView LevelView;
        public CurrencyView CurrencyView;
        public InventoryView InventoryView;
        
        public UI_Panel_Quest UI_Panel_Quest;
        public UI_Panel_Costume UI_Panel_Costume;
        public UI_Panel_Costume_Gacha UI_Panel_Costume_Gacha;
        public UI_Panel_StageMap UI_Panel_StageMap;
        public UI_Panel_LevelUp UI_Panel_LevelUp;
        public UI_Panel_BuildingEnhancement UI_Panel_BuildingEnhancement;
        
        public Button UI_Button_CostumeGachaPanel;
        public Button UI_Button_CostumePanel;
        public Button UI_Button_StageMap;
        public Button UI_Button_SuperHunter;
        public Button UI_Button_CustomerWave;
    }
}
