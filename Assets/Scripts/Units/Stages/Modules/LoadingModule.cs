using UnityEngine;
using UnityEngine.UI;

namespace Units.Stages.Modules
{
    public class LoadingModule : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public Slider Slider => slider;
    }
}