using System;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Modules.TradeZones.Units
{
    public class InteractionModule : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerExit2DAction?.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTriggerStay2DAction?.Invoke(other);
        }

        public event Action<Collider2D> OnTriggerStay2DAction;
        public event Action<Collider2D> OnTriggerExit2DAction;
    }
}