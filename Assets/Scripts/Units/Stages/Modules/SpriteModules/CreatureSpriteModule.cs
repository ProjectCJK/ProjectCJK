using ScriptableObjects.Scripts.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Modules.SpriteModules
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

        public void SetSprites(CreatureSprite creatureSprite)
        {
            _body.sprite = creatureSprite.Body.Count > 0 ? creatureSprite.Body[0] : null;
            _bag.sprite = creatureSprite.Bag.Count > 0 ? creatureSprite.Bag[0] : null;
            _head.sprite = creatureSprite.Head.Count > 0 ? creatureSprite.Head[0] : null;
            _backHair.sprite = creatureSprite.BackHair.Count > 0 ? creatureSprite.BackHair[0] : null;
            _scarf.sprite = creatureSprite.Scarf.Count > 0 ? creatureSprite.Scarf[0] : null;
            _hat.sprite = creatureSprite.Hat.Count > 0 ? creatureSprite.Hat[0] : null;
            _legLeft.sprite = creatureSprite.Leg_Left.Count > 0 ? creatureSprite.Leg_Left[0] : null;
            _legRight.sprite = creatureSprite.Leg_Right.Count > 0 ? creatureSprite.Leg_Right[0] : null;
        }
    }
}