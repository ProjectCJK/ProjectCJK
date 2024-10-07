using System;
using Externals.Joystick.Scripts.Base;
using ScriptableObjects.Scripts;
using Units.Creatures.Abstract;
using Units.Creatures.Units.Players;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Creatures.Controllers
{
    public interface IPlayerFactory
    {
        BaseCreature CreatePlayer(SpawnData playerSpawnData, Joystick joystick, Action<int?, bool> action);
    }

    public class PlayerFactory : IPlayerFactory
    {
        public BaseCreature CreatePlayer(SpawnData playerSpawnData, Joystick joystick, Action<int?, bool> action)
        {
            GameObject player = Object.Instantiate(playerSpawnData.prefab, playerSpawnData.position, Quaternion.identity);
            var baseCreature = player.GetComponent<BaseCreature>();

            if (baseCreature is Player playerCreature)
            {
                playerCreature.RegisterReference(joystick, action);
            }

            return baseCreature;
        }
    }
}