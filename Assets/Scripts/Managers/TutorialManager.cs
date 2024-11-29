using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modules.DesignPatterns.Singletons;
using UI.MainPanels;
using UI.QuestPanels;
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

        private UI_Panel_MainButtons UI_Panel_MainButtons;
        private UI_Panel_Quest UI_Panel_Quest;
        private List<UI_Item_QuestGuide> UI_Item_QuestGuide;
        
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
            
            OnActivateUIByCurrentTutorialIndex += HandleOnActivateUIByCurrentTutorialIndex;

            foreach (KeyValuePair<int, bool> popUpTutorialClear in GameManager.Instance.ES3Saver.PopUpTutorialClear)
            {
                OnActivateUIByCurrentTutorialIndex?.Invoke(popUpTutorialClear.Key);
            }
            
            _branchCanvasGame = UIManager.Instance.BranchCanvasGame;
            _branchCanvasTutorial = UIManager.Instance.BranchCanvasTutorial;
            _branchCanvasGuide = UIManager.Instance.BranchCanvasGuide;
            _branchCanvasJoystick = UIManager.Instance.BranchCanvasJoystick;
            
            _tutorialPanel = UIManager.Instance.UI_Panel_Tutorial;
            _tutorialPopUpPanel = UIManager.Instance.UI_Panel_Tutorial_PopUp;

            UI_Panel_MainButtons = UIManager.Instance.UI_Panel_Main.UI_Panel_MainButtons;
            UI_Panel_Quest = UIManager.Instance.UI_Panel_Main.UI_Panel_Quest;
            UI_Item_QuestGuide = UIManager.Instance.UI_Panel_Main.UI_Item_QuestGuide;
            
            _tutorialPanel.RegisterReference();
            _tutorialPanel.OnClickExitButton += HandleOnClickTutorialPanelExitButton;
            
            _tutorialPopUpPanel.RegisterReference();
            _tutorialPopUpPanel.OnClickExitButton += HandleOnClickPopUpTutorialExitButton;
        }

        public IEnumerator TutorialInitialDialog()
        {
            TransferJoystickCanvas(false);
            TransferGameCanvas(false);
            TransferGuideCanvas(false);
            TransferTutorialCanvas(true);
            
            _tutorialPanel.Initialize();
            
            isScriptEnded = false;

            if (!GameManager.Instance.ES3Saver.first_camera_complete)
            {
                GameManager.Instance.ES3Saver.first_camera_complete = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("first_camera_complete");
            }
            
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
            
            if (!GameManager.Instance.ES3Saver.first_tutorial_popup_tap)
            {
                GameManager.Instance.ES3Saver.first_tutorial_popup_tap = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("first_tutorial_popup_tap");
            }
            
            _tutorialPopUpPanel.gameObject.SetActive(false);
            _branchCanvasGame.gameObject.SetActive(true);
            _branchCanvasJoystick.gameObject.SetActive(true);
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
            if (GameManager.Instance.ES3Saver.PopUpTutorialClear.ContainsKey(index)) return;
         
            OnActivateUIByCurrentTutorialIndex?.Invoke(index);
            GameManager.Instance.ES3Saver.PopUpTutorialClear.TryAdd(index, false);
            
            if (index is 2 or 3 or 4 or 5 or 6 or 7 or 8 or 9 or 48 or 54)
            {
                _tutorialPopUpPanel.ActivatePanel(index);
                GameManager.Instance.ES3Saver.PopUpTutorialClear.TryAdd(index, true);
            }
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

        private void HandleOnActivateUIByCurrentTutorialIndex(int index)
        {
            if (GameManager.Instance.ES3Saver.CurrentStageLevel != 2)
            {
                foreach (GameObject item in UI_Item_QuestGuide.SelectMany(items => items.items))
                {
                    item.gameObject.SetActive(false);
                }
                
                for (var i = 0; i < UI_Item_QuestGuide.Count; i++)
                {
                    if (UI_Item_QuestGuide[i].index == index)
                    {
                        foreach (GameObject item in UI_Item_QuestGuide[i].items)
                        {
                            item.gameObject.SetActive(true);
                        }
                    }
                }
            }

            if (index == 7)
            {
                
            }

            switch (index)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    UI_Panel_MainButtons.UI_Button_CostumeGacha.gameObject.SetActive(true);
                    break;
                case 10:
                    UI_Panel_MainButtons.UI_Button_CostumeGacha.gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_CostumePanel.gameObject.SetActive(true);

                    UI_Panel_MainButtons.UI_Button_LockButton[0].gameObject.SetActive(false);
                    UI_Panel_MainButtons.UI_Button_LockButton[1].gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_LockButton[2].gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_LockButton[3].gameObject.SetActive(true);

                    break;
                case 15:
                    UI_Panel_MainButtons.UI_Button_CostumeGacha.gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_CostumePanel.gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_StageMap.gameObject.SetActive(true);

                    UI_Panel_MainButtons.UI_Button_LockButton[0].gameObject.SetActive(false);
                    UI_Panel_MainButtons.UI_Button_LockButton[1].gameObject.SetActive(false);
                    UI_Panel_MainButtons.UI_Button_LockButton[2].gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_LockButton[3].gameObject.SetActive(true);

                    break;
                default:
                    UI_Panel_MainButtons.UI_Button_CostumeGacha.gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_CostumePanel.gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_StageMap.gameObject.SetActive(true);

                    UI_Panel_MainButtons.UI_Button_LockButton[0].gameObject.SetActive(false);
                    UI_Panel_MainButtons.UI_Button_LockButton[1].gameObject.SetActive(false);
                    UI_Panel_MainButtons.UI_Button_LockButton[2].gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_LockButton[3].gameObject.SetActive(true);
                    break;
            }
        }
    }
}