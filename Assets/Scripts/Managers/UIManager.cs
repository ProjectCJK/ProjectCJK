using System.Collections.Generic;
using Externals.Joystick.Scripts.Base;
using Modules.DesignPatterns.Singletons;
using UI;
using UI.BuildingEnhancementPanel;
using UI.BuildingEnhancementPanel.UI;
using UI.CostumeGachaPanels;
using UI.CostumePanels;
using UI.CurrencyPanel;
using UI.QuestPanels;
using UI.StageMapPanel;
using Units.Stages.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : SingletonMono<UIManager>
    {
        [SerializeField] private Canvas _rootCanvas;
        [SerializeField] private GameObject _eventSystemPrefab;
        [Space(20), SerializeField] private Joystick _joystick;
        
        [Space(20), SerializeField] private Button _ui_Button_CostumeGachaPanel;
        [SerializeField] private Button _ui_Button_CostumePanel;
        [SerializeField] private Button _ui_Button_StageMap;
        
        [Space(20), SerializeField] private UI_Panel_Quest _ui_Panel_Quest;
        [SerializeField] private UI_Panel_BuildingEnhancement _ui_Panel_BuildingEnhancement;
        [SerializeField] private UI_Panel_Costume _ui_Panel_Costume;
        [SerializeField] private UI_Panel_Currency _ui_Panel_Currency;
        [SerializeField] private UI_Panel_Costume_Gacha _ui_Panel_CostumeGacha;
        [SerializeField] private UI_Panel_StageMap _ui_Panel_StageMap;
        
        [Space(20), SerializeField] private GameSpeedControlModule _gameSpeedControlModule;

        public Button UI_Button_CostumeGachaPanel { get; private set; }
        public Button UI_Button_CostumePanel { get; private set; }
        public Button UI_Button_StageMap { get; private set; }
        
        public UI_Panel_Quest UI_Panel_Quest { get; private set; }
        public UI_Panel_BuildingEnhancement UI_Panel_BuildingEnhancement { get; private set; }
        public UI_Panel_Costume UI_Panel_Costume { get; private set; }
        public UI_Panel_Currency UI_Panel_Currency { get; private set; }
        public UI_Panel_Costume_Gacha UI_Panel_CostumeGacha { get; private set; }
        public UI_Panel_StageMap UI_Panel_StageMap { get; private set; }
        public Joystick Joystick { get; private set; }
        private Canvas Canvas { get; set; }

        public void RegisterReference()
        {
            Canvas = Instantiate(_rootCanvas);
            Canvas.GetComponent<Canvas>().worldCamera = Camera.main;
            Instantiate(_eventSystemPrefab);
            
            Joystick = Instantiate(_joystick, Canvas.transform);
            UI_Panel_Currency = Instantiate(_ui_Panel_Currency, Canvas.transform);
            UI_Panel_Quest = Instantiate(_ui_Panel_Quest, Canvas.transform);
            
            UI_Button_CostumeGachaPanel = Instantiate(_ui_Button_CostumeGachaPanel, Canvas.transform);
            UI_Button_StageMap = Instantiate(_ui_Button_StageMap, Canvas.transform);
            UI_Button_CostumePanel = Instantiate(_ui_Button_CostumePanel, Canvas.transform);
            
            UI_Panel_Costume = Instantiate(_ui_Panel_Costume, Canvas.transform);
            UI_Panel_BuildingEnhancement = Instantiate(_ui_Panel_BuildingEnhancement, Canvas.transform);
            UI_Panel_CostumeGacha = Instantiate(_ui_Panel_CostumeGacha, Canvas.transform);
            UI_Panel_StageMap = Instantiate(_ui_Panel_StageMap, Canvas.transform);

            Instantiate(_gameSpeedControlModule, Canvas.transform);
            
            // 부모 자식 관계 정리
            UI_Button_CostumeGachaPanel.transform.localScale = Vector3.one;
            UI_Button_CostumePanel.transform.localScale = Vector3.one;
            UI_Button_StageMap.transform.localScale = Vector3.one;

            UI_Panel_Quest.transform.localScale = Vector3.one;
            UI_Panel_BuildingEnhancement.transform.localScale = Vector3.one;
            UI_Panel_Costume.transform.localScale = Vector3.one;
            UI_Panel_Currency.transform.localScale = Vector3.one;
            UI_Panel_CostumeGacha.transform.localScale = Vector3.one;
            UI_Panel_StageMap.transform.localScale = Vector3.one;
        }
    }
}