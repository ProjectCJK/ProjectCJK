using Externals.Joystick.Scripts.Base;
using Interfaces;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IPlayerController : IInitializable
    {
        public Player Player { get; }
    }
    
    public class PlayerController : IPlayerController
    {
        public Player Player { get; }

        public PlayerController(Transform playerSpawnPoint, Joystick joystick, IItemController itemController)
        {
            IPlayerFactory playerFactory = new PlayerFactory(playerSpawnPoint.position, joystick, itemController);
            Player = playerFactory.CreatePlayer();
        }

        public void Initialize()
        {
            Player.Initialize();
        }
    }
}