using UnityEngine;

namespace Units.Stages.Modules
{
    public class CreatureSpriteModule : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private SpriteRenderer _bag;
        [SerializeField] private SpriteRenderer _head;
        [SerializeField] private SpriteRenderer _backHair;
        [SerializeField] private SpriteRenderer _scarf;
        [SerializeField] private SpriteRenderer _hat;
        [SerializeField] private SpriteRenderer _legLeft;
        [SerializeField] private SpriteRenderer _legRight;

        public void SetSprites(
            Sprite body,
            Sprite bag,
            Sprite head,
            Sprite backHair,
            Sprite scarf,
            Sprite hat,
            Sprite legLeft,
            Sprite legRight)
        {
            _body.sprite = body;
            _bag.sprite = bag;
            _head.sprite = head;
            _backHair.sprite = backHair;
            _scarf.sprite = scarf;
            _hat.sprite = hat;
            _legLeft.sprite = legLeft;
            _legRight.sprite = legRight;
        }
    }
}