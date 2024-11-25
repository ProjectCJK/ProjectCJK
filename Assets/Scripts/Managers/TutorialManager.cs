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
        private Action OnCameraMovementEnded;
        
        private Canvas _branchCanvasGame;
        private Canvas _branchCanvasTutorial;
        private Canvas _branchCanvasGuide;
        private Canvas _branchCanvasJoystick;
        
        private UI_Panel_Tutorial _tutorialPanel;
        private UI_Panel_PopUpTutorial _popUpTutorialPanel;
        
        private Vector3 _zombieZonePosition;
        private Vector3 _huntingZonePosition;
        
        private CameraController _cameraController;

        private List<Vector3> _firstTargets;
        private List<Vector3> _secondTargets;

        private float _zPosition;
        private bool isScriptEnded;
        
        public void RegisterReference(CameraController cameraController, Action onCameraMovementEnded)
        {
            GameManager.Instance.ES3Saver.PopUpTutorialClear = new ();
            
            OnCameraMovementEnded = onCameraMovementEnded;
            
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

            _cameraController.transform.position = _firstTargets[0];
            
            UIManager.Instance.InstantiateTutorialPanel();
            
            _branchCanvasGame = UIManager.Instance.BranchCanvasGame;
            _branchCanvasTutorial = UIManager.Instance.BranchCanvasTutorial;
            _branchCanvasGuide = UIManager.Instance.BranchCanvasGuide;
            _branchCanvasJoystick = UIManager.Instance.BranchCanvasJoystick;
            
            _tutorialPanel = UIManager.Instance.UI_Panel_Tutorial;
            _popUpTutorialPanel = UIManager.Instance.UI_Panel_PopUpTutorial;
            
            _tutorialPanel.RegisterReference();
            _tutorialPanel.OnScriptsEnded += HandleOnScriptsEnded;
            
            _popUpTutorialPanel.RegisterReference();
            _popUpTutorialPanel.OnClickExitButton += HandleOnClickExitButton;
        }

        public void Initialize()
        {
            TransferTutorialCanvas(true);
            
            _tutorialPanel.Initialize();
            CoroutineManager.Instance.StartManagedCoroutine(Tutorial());
        }

        private IEnumerator Tutorial()
        {
            isScriptEnded = false;
            var firstTarget = false;
            var secondTarget = false;
            
            _cameraController.FollowTempTarget(_firstTargets[1], () => firstTarget = true);

            while (!firstTarget)
            {
                yield return null;   
            }
            
            _cameraController.transform.position = _secondTargets[0];
            _cameraController.FollowTempTarget(_secondTargets[1], () => secondTarget = true);

            while (!secondTarget)
            {
                yield return null;   
            }
            
            _cameraController.UnregisterReference();
            OnCameraMovementEnded?.Invoke();
            
            _tutorialPanel.gameObject.SetActive(true);

            while (!isScriptEnded)
            {
                yield return null;
            }
            
            _tutorialPanel.gameObject.SetActive(false);
            
            ActivePopUpTutorialPanel(0);
        }

        public void ActivePopUpTutorialPanel(int index)
        {
            if (GameManager.Instance.ES3Saver.PopUpTutorialClear.TryGetValue(index, out var value))
            {
                if (value) return;
                
                GameManager.Instance.ES3Saver.PopUpTutorialClear[index] = true;

                TransferTutorialCanvas(true);
            
                _popUpTutorialPanel.ActivatePanel(index);
            }
        }

        private void HandleOnScriptsEnded()
        {
            GameManager.Instance.ES3Saver.TutorialClear = true;
            isScriptEnded = true;
        }
        
        private void HandleOnClickExitButton()
        {
            TransferTutorialCanvas(false);
        }

        private void TransferTutorialCanvas(bool value)
        {
            _branchCanvasJoystick.gameObject.SetActive(!value);
            _branchCanvasGame.gameObject.SetActive(!value);
            _branchCanvasGuide.gameObject.SetActive(!value);
            _branchCanvasTutorial.gameObject.SetActive(value);
        }
    }
}