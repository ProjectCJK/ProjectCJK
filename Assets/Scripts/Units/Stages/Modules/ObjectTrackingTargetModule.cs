using System.Collections.Generic;
using UnityEngine;

namespace Units.Stages.Modules
{
    public class ObjectTrackingTargetModule : MonoBehaviour
    {
        [SerializeField] private List<Transform> _target;
        public List<Transform> Target => _target;
    }
}