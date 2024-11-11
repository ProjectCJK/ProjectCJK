using ScriptableObjects.Scripts.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Modules.SpriteModules
{
    public class MonsterSpriteModule : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _emotionIdleSprite;
        [SerializeField] private SpriteRenderer _emotionScaredSprite;
        [SerializeField] private SpriteRenderer _emotionDeathSprite;
        [SerializeField] private SpriteRenderer _bodySprite;
        [SerializeField] private SpriteRenderer _bodyDeathSprite;
        [SerializeField] private SpriteRenderer _hairSprite;
        [SerializeField] private SpriteRenderer _legLeftSprite;
        [SerializeField] private SpriteRenderer _legRightSprite;

        public void SetSprites(MonsterSprite monsterSprite)
        {
            _emotionIdleSprite.sprite = monsterSprite.EmotionIdleSprite;
            _emotionScaredSprite.sprite = monsterSprite.EmotionScaredSprite;
            _emotionDeathSprite.sprite = monsterSprite.EmotionDeathSprite;
            _bodySprite.sprite = monsterSprite.BodySprite;
            _bodyDeathSprite.sprite = monsterSprite.BodyDeathSprite;
            _hairSprite.sprite = monsterSprite.HairSprite;
            _legLeftSprite.sprite = monsterSprite.LegLeftSprite;
            _legRightSprite.sprite = monsterSprite.LegRightSprite;
        }
    }
}