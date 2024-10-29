using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Modules
{
    public interface IDamageFlashModule : IRegisterReference
    {
        public void ActivateEffects();
    }
    
    public class DamageFlashModule : MonoBehaviour, IDamageFlashModule
    {
        public Color flashColor = Color.red; // 데미지 시 플래시 색상
        public float flashDuration = 0.1f; // 깜빡임 지속 시간

        private List<SpriteRenderer> spriteRenderers; // 하위 SpriteRenderer들을 저장할 리스트
        private Dictionary<SpriteRenderer, Color> originalColors; // 각 SpriteRenderer의 원래 색상 저장용
        private bool isFlashing; // 플래시 효과가 진행 중인지 체크하는 변수
        private float flashTimer; // 플래시 지속 시간을 측정할 타이머

        public void RegisterReference()
        {
            spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
            originalColors = new Dictionary<SpriteRenderer, Color>();

            foreach (var spriteRenderer in spriteRenderers)
            {
                // 각 SpriteRenderer의 원래 색상을 저장
                originalColors[spriteRenderer] = spriteRenderer.color;
            }
        }

        private void Update()
        {
            // 플래시 효과가 활성화되었을 때 타이머를 체크
            if (isFlashing)
            {
                flashTimer += Time.deltaTime;
                if (flashTimer >= flashDuration)
                {
                    ResetFlash();
                }
            }
        }

        public void ActivateEffects()
        {
            if (!isFlashing)
            {
                StartFlash();
            }
        }

        private void StartFlash()
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = flashColor;
            }
            isFlashing = true;
            flashTimer = 0f;
        }

        private void ResetFlash()
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = originalColors[spriteRenderer];
            }
            isFlashing = false;
        }
    }
}