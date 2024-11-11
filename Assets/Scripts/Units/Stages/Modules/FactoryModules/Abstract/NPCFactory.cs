using System.Collections.Generic;
using ScriptableObjects.Scripts.Creatures.Units;
using UnityEngine;
using Random = System.Random;

namespace Units.Stages.Modules.FactoryModules.Abstract
{
    public abstract class NPCFactory : Factory
    {
        protected CreatureSprite GetRandomSprites(CreatureSprite creatureSprite)
        {
            var randomSprites = new CreatureSprite();
            var random = new Random();

            // 1. Head와 BackHair는 동일한 인덱스에서 랜덤 선택
            var headIndex = GetRandomIndex(creatureSprite.Head, random);
            if (headIndex != -1)
            {
                randomSprites.Head = new List<Sprite> { creatureSprite.Head[headIndex] };
                // BackHair가 해당 인덱스에 존재하는지 확인하고 추가
                randomSprites.BackHair = creatureSprite.BackHair.Count > headIndex
                    ? new List<Sprite> { creatureSprite.BackHair[headIndex] }
                    : new List<Sprite>();
            }
            else
            {
                randomSprites.Head = new List<Sprite>();
                randomSprites.BackHair = new List<Sprite>();
            }

            // 2. Body와 Leg_Left, Leg_Right는 동일한 인덱스에서 랜덤 선택
            var bodyIndex = GetRandomIndex(creatureSprite.Body, random);
            if (bodyIndex != -1)
            {
                randomSprites.Body = new List<Sprite> { creatureSprite.Body[bodyIndex] };
                randomSprites.Leg_Left = creatureSprite.Leg_Left.Count > bodyIndex
                    ? new List<Sprite> { creatureSprite.Leg_Left[bodyIndex] }
                    : new List<Sprite>();
                randomSprites.Leg_Right = creatureSprite.Leg_Right.Count > bodyIndex
                    ? new List<Sprite> { creatureSprite.Leg_Right[bodyIndex] }
                    : new List<Sprite>();
            }
            else
            {
                randomSprites.Body = new List<Sprite>();
                randomSprites.Leg_Left = new List<Sprite>();
                randomSprites.Leg_Right = new List<Sprite>();
            }

            // 3. Bag, Scarf, Hat은 개별적으로 랜덤 선택
            randomSprites.Bag = GetRandomSprite(creatureSprite.Bag, random);
            randomSprites.Scarf = GetRandomSprite(creatureSprite.Scarf, random);
            randomSprites.Hat = GetRandomSprite(creatureSprite.Hat, random);

            return randomSprites;
        }

        private int GetRandomIndex(List<Sprite> sprites, Random random)
        {
            return sprites.Count > 0 ? random.Next(sprites.Count) : -1;
        }

        private List<Sprite> GetRandomSprite(List<Sprite> sprites, Random random)
        {
            return sprites is { Count: > 0 }
                ? new List<Sprite> { sprites[random.Next(sprites.Count)] }
                : new List<Sprite>();
        }
    }
}