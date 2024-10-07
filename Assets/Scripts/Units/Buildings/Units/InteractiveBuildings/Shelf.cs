using Units.Buildings.Interfaces;
using Units.Buildings.Modules;
using Units.Buildings.Units.Abstract;
using UnityEngine;

namespace Units.Buildings.Units.InteractiveBuildings
{
    [RequireComponent(typeof(InteractionTrade))]
    public class Shelf : InteractiveBuilding
    {
        private InteractionTrade _interactionTrade;
        
        public override void RegisterReference()
        {
            _interactionTrade = GetComponent<InteractionTrade>();
        }

        public override void Initialize()
        {
            _interactionTrade.OnSendObject += SendObject;
            _interactionTrade.OnReceiveObject += ReceiveObject;
        }
        
        public void SendObject()
        {
            
        }

        public void ReceiveObject()
        {
            
        }
    }
}