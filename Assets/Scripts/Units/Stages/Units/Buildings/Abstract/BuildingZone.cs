using Interfaces;
using Units.Stages.Units.Buildings.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Abstract
{
    public interface IBuildingZone : IInitializable
    {
        public string BuildingKey { get; }
        public string InputItemKey { get; }
        public string OutputItemKey { get; }

        public Transform TradeZoneNpcTransform { get; }
    }

    public abstract class BuildingZone : MonoBehaviour, IBuildingZone
    {
        public abstract string BuildingKey { get; protected set; }
        public abstract string InputItemKey { get; protected set; }
        public abstract string OutputItemKey { get; protected set; }
        public abstract Animator Animator { get; }
        public abstract Transform TradeZoneNpcTransform { get; }

        public abstract void Initialize();
        
        public void HandleOnTriggerBuildingAnimation(EBuildingAnimatorParameter animatorParameter)
        {
            Animator.SetTrigger($"{animatorParameter}");
        }
    }
}