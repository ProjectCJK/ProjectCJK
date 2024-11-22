using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Managers;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.StageMapPanel
{
    public class UI_Panel_StageMap : MonoBehaviour
    {
        [SerializeField] private List<UI_Item_StageMap> _uiItemStageMaps;
        private List<StageData> _stageDatas;
        
        public void RegisterReference(List<StageData> stageDataList, Action<int, int> changeStage)
        {
            _stageDatas = stageDataList;
            
            for (var index = 0; index < _uiItemStageMaps.Count; index++)
            {
                UI_Item_StageMap uiItemStageMap = _uiItemStageMaps[index];
                uiItemStageMap.RegisterReference(() => changeStage(index, _stageDatas[index].UnlockCost));
            }
        }
        
        public void Activate()
        {
            for (var index = 0; index < _uiItemStageMaps.Count; index++)
            {
                UI_Item_StageMap stageMap = _uiItemStageMaps[index];
                stageMap.UpdateUI(_stageDatas[index]);
            }
            
            gameObject.SetActive(true);
        }
    }
}