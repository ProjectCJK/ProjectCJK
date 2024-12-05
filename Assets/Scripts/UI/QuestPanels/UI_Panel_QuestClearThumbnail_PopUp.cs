using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using UnityEngine;

namespace UI.QuestPanels
{
    public class UI_Panel_QuestClearThumbnail_PopUp : MonoBehaviour
    {
        [SerializeField] private UI_Panel_QuestClearThumbnail_PopUp_Item item;
        
        private const string TAG = "UI_Panel_QuestClearThumbnail_PopUp";
        
        public void RegisterReference()
        {
            ObjectPoolManager.Instance.CreatePool(TAG, 5, 5, true, () =>
            {
                UI_Panel_QuestClearThumbnail_PopUp_Item obj = Instantiate(item, transform);
                obj.RegisterReference(() => ObjectPoolManager.Instance.ReturnObject(TAG, obj));
                return obj;
            });
        }

        public void GetQuestClearThumbnailPopUp(UIQuestInfoItem questInfoItem)
        {
            var obj = ObjectPoolManager.Instance.GetObject<UI_Panel_QuestClearThumbnail_PopUp_Item>(TAG, null);
            
            obj.UpdateClearThumbnailQuestPopUp(questInfoItem);
        }
    }
}