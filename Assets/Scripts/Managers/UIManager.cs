using Externals.Joystick.Scripts.Base;
using Modules.DesignPatterns.Singletons;
using UI.MainPanels;
using UI.TutorialPanel;
using Units.Stages.Modules;
using UnityEngine;

namespace Managers
{
    public class UIManager : SingletonMono<UIManager> 
    {
        [SerializeField] private GameObject _eventSystemPrefab;
        [SerializeField] private Canvas _rootCanvas;
        [SerializeField] private Canvas _branchCanvasJoystick;
        [SerializeField] private Canvas _branchCanvasGame;
        [SerializeField] private Canvas _branchCanvasTutorial;
        [SerializeField] private Canvas _branchCanvasGuide;
        
        [Space(20), SerializeField] private Joystick _joystick;
        
        [Space(20), SerializeField] private UI_Panel_Main _ui_Panel_Main;
        [SerializeField] private UI_Panel_Tutorial_Initial uiPanelTutorialInitial;
        [SerializeField] private UI_Panel_Tutorial_PopUp _uiPanelTutorialPopUp;
        [SerializeField] private UI_Panel_Tutorial_Joystick _uiPanelTutorialJoystick;
        
        [Space(20), SerializeField] private GameSpeedControlModule _gameSpeedControlModule;
        
        public UI_Panel_Main UI_Panel_Main { get; private set; }
        public UI_Panel_Tutorial_Initial UIPanelTutorialInitial { get; private set; }
        public UI_Panel_Tutorial_PopUp UI_Panel_Tutorial_PopUp { get; private set; }
        public UI_Panel_Tutorial_Joystick UI_Panel_Tutorial_Joystick { get; private set; }
        public Joystick Joystick { get; private set; }
        
        /// <summary>
        /// Canvas Settings
        /// </summary>
        public Canvas RootCanvas { get; set; }  
        public Canvas BranchCanvasGuide { get; set; }
        public Canvas BranchCanvasGame { get; set; }
        public Canvas BranchCanvasTutorial { get; set; }
        public Canvas BranchCanvasJoystick { get; set; }
        
        public void RegisterReference()
        {
            // RootCanvas = Instantiate(_rootCanvas);
            // RootCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            
            Instantiate(_eventSystemPrefab);
            
            BranchCanvasJoystick = Instantiate(_branchCanvasJoystick);
            var branchCanvasJoystick = BranchCanvasJoystick.GetComponent<Canvas>();
            branchCanvasJoystick.worldCamera = Camera.main;
            branchCanvasJoystick.sortingLayerName = "UI";
            branchCanvasJoystick.sortingOrder = 50;
            
            BranchCanvasGuide = Instantiate(_branchCanvasGuide);
            var branchCanvasGuide = BranchCanvasGuide.GetComponent<Canvas>();
            branchCanvasGuide.worldCamera = Camera.main;
            branchCanvasGuide.sortingLayerName = "UI";
            branchCanvasGuide.sortingOrder = 100;
            
            BranchCanvasGame = Instantiate(_branchCanvasGame);
            var branchCanvasGame = BranchCanvasGame.GetComponent<Canvas>();
            branchCanvasGame.worldCamera = Camera.main;
            branchCanvasGame.sortingLayerName = "UI";
            branchCanvasGame.sortingOrder = 150;
            
            Joystick = Instantiate(_joystick, BranchCanvasJoystick.transform);
            UI_Panel_Main = Instantiate(_ui_Panel_Main, BranchCanvasGame.transform);
            
            BranchCanvasTutorial = Instantiate(_branchCanvasTutorial);
            UIPanelTutorialInitial = Instantiate(uiPanelTutorialInitial, BranchCanvasTutorial.transform);
            UI_Panel_Tutorial_PopUp = Instantiate(_uiPanelTutorialPopUp, BranchCanvasTutorial.transform);
            UI_Panel_Tutorial_Joystick = Instantiate(_uiPanelTutorialJoystick, BranchCanvasTutorial.transform);
   
            // Instantiate(_gameSpeedControlModule, BranchCanvasGame.transform);
        }
    }
}