using Externals.Joystick.Scripts.Base;
using ScriptableObjects.Scripts;
using Units.Games.Creatures.Abstract;
using Units.Games.Creatures.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Games.Creatures.Controllers
{
    public interface ICreatureFactory
    {
        Creature CreateCreature();
    }

    public class CreatureFactory : ICreatureFactory
    {
        private readonly PlayerStatSO _playerStatSo;
        private readonly Joystick _joystick;
        
        public CreatureFactory(PlayerStatSO playerStatSo, Joystick joystick)
        {
            _playerStatSo = playerStatSo;
            _joystick = joystick;
        }

        public Creature CreateCreature()
        {
            // TODO : NPC SO 추가 후 로직 수정
            GameObject prefab = _playerStatSo.BaseSpawnData.Prefab;
            Vector3Int spawnPos = _playerStatSo.BaseSpawnData.Position;
            
            GameObject obj = Object.Instantiate(prefab, spawnPos, Quaternion.identity);
            var baseCreature = obj.GetComponent<Creature>();
            
            var player = baseCreature as Player;
            
            if (player != null)
            {
                player.RegisterReference(_playerStatSo, _joystick);
                player.RegisterEventListener();
            }

            return baseCreature;
        }
    }
}