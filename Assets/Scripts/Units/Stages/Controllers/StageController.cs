using Externals.Joystick.Scripts.Base;
using Interfaces;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IStageController : IRegisterReference<Joystick, IItemController>, IInitializable
    {
        public Transform GetPlayerTransform();
    }
    
    public class StageController : MonoBehaviour, IStageController
    {
        [SerializeField] private CreatureController _creatureController;
        [SerializeField] private BuildingController _buildingController;
        
        public void RegisterReference(Joystick joystick, IItemController itemController)
        {
            _creatureController.RegisterReference(joystick, itemController);
            _buildingController.RegisterReference(itemController);
        }
        
        public void Initialize()
        {
            _creatureController.Initialize();
            _buildingController.Initialize();
        }

        public Transform GetPlayerTransform() => _creatureController.GetPlayerTransform();
    }
}