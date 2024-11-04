using UnityEngine;

namespace Units.Stages.Modules
{
    public static class LayerMaskParserModule
    {
        private const string monsterCollisionLayer = "MonsterCollision";
        private const string unitLayer = "Unit";
        private const string collisionLayer = "Collision";
        
        public static readonly int CollisionLayerMask = LayerMask.GetMask(collisionLayer);
        public static readonly int MonsterCollisionLayerMask = LayerMask.GetMask(monsterCollisionLayer);
        public static readonly int UnitLayerMask = LayerMask.GetMask(unitLayer);
    }
}