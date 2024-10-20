using UnityEngine;
using UnityEngine.EventSystems;

namespace Externals.Joystick.Scripts.Units
{
    public class FloatingJoystick : Base.Joystick
    {
        protected override void Start()
        {
            base.Start();
            background.gameObject.SetActive(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background.parent as RectTransform, // 부모 RectTransform 기준
                eventData.position,
                eventData.pressEventCamera, // 사용 중인 카메라
                out Vector2 anchoredPos); // 변환된 위치 반환

            background.anchoredPosition = anchoredPos;
            background.gameObject.SetActive(true);

            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            background.gameObject.SetActive(false);
            base.OnPointerUp(eventData);
        }
    }
}