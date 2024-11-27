using System;
using System.Collections;
using System.Collections.Generic;
using Modules.DesignPatterns.Singletons;
using UI.TutorialPanel;
using Units.Stages.Controllers;
using UnityEngine;

namespace Managers
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        public Action<int> OnActivateUIByCurrentTutorialIndex;
        
        private Canvas _branchCanvasGame;
        private Canvas _branchCanvasTutorial;
        private Canvas _branchCanvasGuide;
        private Canvas _branchCanvasJoystick;
        
        private UI_Panel_Tutorial _tutorialPanel;
        private UI_Panel_Tutorial_PopUp _tutorialPopUpPanel;
        
        private Vector3 _zombieZonePosition;
        private Vector3 _huntingZonePosition;
        
        private CameraController _cameraController;

        private List<Vector3> _firstTargets;
        private List<Vector3> _secondTargets;

        private float _zPosition;
        private bool isScriptEnded;
        
        public void RegisterReference(CameraController cameraController)
        {
            GameManager.Instance.ES3Saver.PopUpTutorialClear = new Dictionary<int, bool>();
            UIManager.Instance.UI_Panel_Main.RegisterReference();
            
            _cameraController = cameraController;
            _zPosition = _cameraController.transform.position.z;

            _firstTargets = new List<Vector3>
            {
                new(17, -1, _zPosition),
                new(24, -1, _zPosition)
            };
            
            _secondTargets = new List<Vector3>
            {
                new(9, 18, _zPosition),
                new(9, 25, _zPosition)
            };
            
            UIManager.Instance.InstantiateTutorialPanel();

            OnActivateUIByCurrentTutorialIndex += UIManager.Instance.UI_Panel_Main.OnActivateUIByCurrentTutorialIndex;

            foreach (KeyValuePair<int, bool> popUpTutorialClear in GameManager.Instance.ES3Saver.PopUpTutorialClear)
            {
                OnActivateUIByCurrentTutorialIndex?.Invoke(popUpTutorialClear.Key);
            }
            
            _branchCanvasGame = UIManager.Instance.BranchCanvasGame;
            _branchCanvasTutorial = UIManager.Instance.BranchCanvasTutorial;
            _branchCanvasGuide = UIManager.Instance.BranchCanvasGuide;
            _branchCanvasJoystick = UIManager.Instance.BranchCanvasJoystick;
            
            _tutorialPanel = UIManager.Instance.UI_Panel_Tutorial;
            _tutorialPopUpPanel = UIManager.Instance.UIPanelTutorialPopUp;
            
            _tutorialPanel.RegisterReference();
            _tutorialPanel.OnClickExitButton += HandleOnClickTutorialPanelExitButton;
            
            _tutorialPopUpPanel.RegisterReference();
            _tutorialPopUpPanel.OnClickExitButton += HandleOnClickPopUpTutorialExitButton;
        }

        public void Initialize()
        {
            TransferJoystickCanvas(false);
            TransferGameCanvas(false);
            TransferGuideCanvas(false);
            TransferTutorialCanvas(true);
            
            _tutorialPanel.Initialize();
            CoroutineManager.Instance.StartManagedCoroutine(TutorialInitialDialog());
        }

        private IEnumerator TutorialInitialDialog()
        {
            isScriptEnded = false;

            // 컷씬 카메라 연출
            yield return PlayCameraCutscenes();
            
            // 컷씬 다이어로그
            yield return PlayInitialCutscenes();

            // 첫 팝업 튜토리얼 다이어로그
            yield return PlayFirstPopUpTutorial();
        }

        private IEnumerator PlayCameraCutscenes()
        {
            yield return FollowCameraToTargets(_firstTargets[0], _firstTargets[1]);
            yield return FollowCameraToTargets(_secondTargets[0], _secondTargets[1]);
        }
        
        private IEnumerator PlayInitialCutscenes()
        {
            _tutorialPanel.gameObject.SetActive(true);

            while (!isScriptEnded)
            {
                yield return null;
            }
            
            _tutorialPanel.gameObject.SetActive(false);
        }

        private IEnumerator PlayFirstPopUpTutorial()
        {
            var cameraTracking = false;
            
            _branchCanvasGuide.gameObject.SetActive(true);

            _cameraController.ActivateFollowCamera(ObjectTrackerManager.Instance.TargetTransform.position, true, () => cameraTracking = true);

            while (!cameraTracking)
            {
                yield return null;
            }
            
            _tutorialPopUpPanel.gameObject.SetActive(true);
            _branchCanvasGame.gameObject.SetActive(true);
            _branchCanvasJoystick.gameObject.SetActive(true);

            ActivePopUpTutorialPanel(0);
        }
        
        private IEnumerator FollowCameraToTargets(Vector3 start, Vector3 end)
        {
            var isComplete = false;

            _cameraController.ActivateFollowCamera(start, end, () => isComplete = true);

            while (!isComplete)
            {
                yield return null;
            }
        }

        public void ActivePopUpTutorialPanel(int index)
        {
            if (GameManager.Instance.ES3Saver.PopUpTutorialClear.ContainsKey(index) && GameManager.Instance.ES3Saver.PopUpTutorialClear[index]) return;

            GameManager.Instance.ES3Saver.PopUpTutorialClear.TryAdd(index, true);
            OnActivateUIByCurrentTutorialIndex?.Invoke(index);
            
            _tutorialPopUpPanel.ActivatePanel(index);
        }

        private void HandleOnClickTutorialPanelExitButton()
        {
            GameManager.Instance.ES3Saver.InitialTutorialClear = true;
            isScriptEnded = true;
        }
        
        private void HandleOnClickPopUpTutorialExitButton(int index)
        {
            GameManager.Instance.ES3Saver.PopUpTutorialClear[index] = true;
        }

        private void TransferJoystickCanvas(bool value)
        {
            _branchCanvasJoystick.gameObject.SetActive(value);
        }
        
        private void TransferGameCanvas(bool value)
        {
            _branchCanvasGame.gameObject.SetActive(value);
        }
        
        private void TransferGuideCanvas(bool value)
        {
            _branchCanvasGuide.gameObject.SetActive(value);
        }
        
        private void TransferTutorialCanvas(bool value)
        {
            _branchCanvasTutorial.gameObject.SetActive(value);
        }
    }
}