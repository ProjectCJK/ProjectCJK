using Managers;
using ScriptableObjects.Scripts.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Modules.SpriteModules
{
    public class CreatureSpriteModule : MonoBehaviour
    {
        public CreatureSprite CreatureSprite;
        
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
            CreatureSprite = creatureSprite;

            if (creatureSprite.Body.Count > 0) _body.sprite = creatureSprite.Body[0];
            if (creatureSprite.Bag.Count > 0) _bag.sprite = creatureSprite.Bag[0];
            if (creatureSprite.Head.Count > 0) _head.sprite = creatureSprite.Head[0];
            if (creatureSprite.BackHair.Count > 0) _backHair.sprite = creatureSprite.BackHair[0];
            if (creatureSprite.Scarf.Count > 0) _scarf.sprite = creatureSprite.Scarf[0];
            if (creatureSprite.Hat.Count > 0) _hat.sprite = creatureSprite.Hat[0];
            if (creatureSprite.Leg_Left.Count > 0) _legLeft.sprite = creatureSprite.Leg_Left[0];
            if (creatureSprite.Leg_Right.Count > 0) _legRight.sprite = creatureSprite.Leg_Right[0];
        }
    }
}