using UnityEngine;

namespace Units.Stages.Modules
{
    public class MonsterSpriteModule : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _emotionIdleSprite;
        [SerializeField] private SpriteRenderer _emotionScaredSprite;
        [SerializeField] private SpriteRenderer _emotionDeathSprite;
        [SerializeField] private SpriteRenderer _bodySprite;
        [SerializeField] private SpriteRenderer _bodyDeathSprite;
        [SerializeField] private SpriteRenderer _hairDeathSprite;
        [SerializeField] private SpriteRenderer _legLeftSprite;
        [SerializeField] private SpriteRenderer _legRightSprite;

        public void SetSprites(
            Sprite emotionIdleSprite,
            Sprite emotionScaredSprite,
            Sprite emotionDeathSprite,
            Sprite bodySprite,
            Sprite bodyDeathSprite,
            Sprite hairDeathSprite,
            Sprite legLeftSprite,
            Sprite legRightSprite)
        {
            _emotionIdleSprite.sprite = emotionIdleSprite;
            _emotionScaredSprite.sprite = emotionScaredSprite;
            _emotionDeathSprite.sprite = emotionDeathSprite;
            _bodySprite.sprite = bodySprite;
            _bodyDeathSprite.sprite = bodyDeathSprite;
            _hairDeathSprite.sprite = hairDeathSprite;
            _legLeftSprite.sprite = legLeftSprite;
            _legRightSprite.sprite = legRightSprite;
        }
    }
}