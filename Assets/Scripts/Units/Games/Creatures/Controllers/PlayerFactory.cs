using Externals.Joystick.Scripts.Base;
using ScriptableObjects.Scripts;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Games.Creatures.Abstract;
using Units.Games.Creatures.Units;
using Units.Games.Items.Controllers;
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
        private readonly PlayerDataSo _playerDataSo;
        private readonly Joystick _joystick;
        private readonly IItemController _itemController;
        
        public CreatureFactory(PlayerDataSo playerDataSo, Joystick joystick, IItemController itemController)
        {
            _playerDataSo = playerDataSo;
            _joystick = joystick;
            _itemController = itemController;
        }

        public Creature CreateCreature()
        {
            // TODO : NPC SO 추가 후 로직 수정
            GameObject prefab = _playerDataSo.BaseSpawnData.Prefab;
            Vector3Int spawnPos = _playerDataSo.BaseSpawnData.Position;
            
            GameObject obj = Object.Instantiate(prefab, spawnPos, Quaternion.identity);
            var baseCreature = obj.GetComponent<Creature>();
            
            var player = baseCreature as Player;
            
            if (player != null)
            {
                player.RegisterReference(_playerDataSo, _joystick, _itemController);
                player.RegisterEventListener();
            }

            return baseCreature;
        }
    }
}