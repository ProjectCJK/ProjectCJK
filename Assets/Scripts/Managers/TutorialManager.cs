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
        
        private UI_Panel_Tutorial _tutorialPanel;
        private Canvas _branchCanvasGame;
        private Canvas _branchCanvasTutorial;
        private Canvas _branchCanvasGuide;

        private Vector3 _zombieZonePosition;
        private Vector3 _huntingZonePosition;
        
        private CameraController _cameraController;

        private List<Vector3> _firstTargets;
        private List<Vector3> _secondTargets;

        private float _zPosition;
        
        public void RegisterReference(CameraController cameraController, Action onCameraMovementEnded)
        {
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

            _tutorialPanel = UIManager.Instance.UI_Panel_Tutorial;
            _branchCanvasGame = UIManager.Instance.BranchCanvasGame;
            _branchCanvasTutorial = UIManager.Instance.BranchCanvasTutorial;
            _branchCanvasGuide = UIManager.Instance.BranchCanvasGuide;
            
            _tutorialPanel.RegisterReference();
            _tutorialPanel.OnScriptsEnded += HandleOnScriptsEnded;
        }

        public void Initialize()
        {
            _branchCanvasGame.gameObject.SetActive(false);
            
            _tutorialPanel.Initialize();
            CoroutineManager.Instance.StartManagedCoroutine(Tutorial());
        }

        private IEnumerator Tutorial()
        {
            var firstTarget = false;
            var secondTarget = false;
            
            _cameraController.FollowTempTarget(_firstTargets[1], () => firstTarget = true);

            while (!firstTarget)
            {
                yield return null;   
            }
            
            // yield return new WaitForSeconds(0.7f);
            
            _cameraController.transform.position = _secondTargets[0];
            _cameraController.FollowTempTarget(_secondTargets[1], () => secondTarget = true);

            while (!secondTarget)
            {
                yield return null;   
            }
            
            // yield return new WaitForSeconds(0.7f);
            
            _cameraController.UnregisterReference();
            OnCameraMovementEnded?.Invoke();
            
            _tutorialPanel.gameObject.SetActive(true);
        }

        private void HandleOnScriptsEnded()
        {
            UIManager.Instance.DestroyTutorialPanel();
            GameManager.Instance.ES3Saver.TutorialClear = true;
            _branchCanvasGame.gameObject.SetActive(true);
        }
    }
}