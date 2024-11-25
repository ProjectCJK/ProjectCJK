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
        [SerializeField] private Canvas _branchCanvasGame;
        [SerializeField] private Canvas _branchCanvasTutorial;
        [SerializeField] private Canvas _branchCanvasGuide;
        [Space(20), SerializeField] private Joystick _joystick;

        [Space(20), SerializeField] private UI_Panel_Main _ui_Panel_Main;

        [SerializeField] private UI_Panel_Tutorial _ui_Panel_Tutorial;
        [SerializeField] private UI_Panel_PopUpTutorial _ui_Panel_PopUpTutorial;
        
        [Space(20), SerializeField] private GameSpeedControlModule _gameSpeedControlModule;

        public UI_Panel_Main UI_Panel_Main { get; private set; }
        public UI_Panel_Tutorial UI_Panel_Tutorial { get; private set; }
        public UI_Panel_PopUpTutorial UI_Panel_PopUpTutorial { get; private set; }
        public Joystick Joystick { get; private set; }
        
        /// <summary>
        /// Canvas Settings
        /// </summary>
        public Canvas RootCanvas { get; set; }
        public Canvas BranchCanvasGuide { get; set; }
        public Canvas BranchCanvasGame { get; set; }
        public Canvas BranchCanvasTutorial { get; set; }

        public void RegisterReference()
        {
            // RootCanvas = Instantiate(_rootCanvas);
            // RootCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            
            Instantiate(_eventSystemPrefab);
            BranchCanvasGuide = Instantiate(_branchCanvasGuide);
            BranchCanvasGame = Instantiate(_branchCanvasGame);
            var branchCanvasGame = BranchCanvasGame.GetComponent<Canvas>();
            branchCanvasGame.worldCamera = Camera.main;
            branchCanvasGame.sortingLayerName = "UI";
            branchCanvasGame.sortingOrder = 10;
            
            Joystick = Instantiate(_joystick, BranchCanvasGame.transform);
            UI_Panel_Main = Instantiate(_ui_Panel_Main, BranchCanvasGame.transform);
   
            Instantiate(_gameSpeedControlModule, BranchCanvasGame.transform);
        }
        
        public void InstantiateTutorialPanel()
        {
            BranchCanvasTutorial = Instantiate(_branchCanvasTutorial);
            UI_Panel_Tutorial = Instantiate(_ui_Panel_Tutorial, BranchCanvasTutorial.transform);
            UI_Panel_PopUpTutorial = Instantiate(_ui_Panel_PopUpTutorial, BranchCanvasTutorial.transform);
        }

        public void DestroyTutorialPanel()
        {
            Destroy(BranchCanvasTutorial);
        }
    }
}