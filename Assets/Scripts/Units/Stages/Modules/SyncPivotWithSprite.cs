using UnityEngine;
using UnityEngine.UI;

namespace Units.Stages.Modules
{
    [RequireComponent(typeof(Image))]
    public class SyncPivotWithSprite : MonoBehaviour
    {
        private void OnEnable()
        {
            var image = GetComponent<Image>();
            
            if (image.sprite != null)
            {
                // Sprite의 Pivot 값을 가져옵니다 (0~1 범위)
                Vector2 spritePivot = image.sprite.pivot / image.sprite.rect.size;

                // RectTransform의 Pivot 값을 Sprite의 Pivot과 동기화
                RectTransform rectTransform = image.rectTransform;
                rectTransform.pivot = spritePivot;
                image.SetNativeSize();
            }
        }
    }
}