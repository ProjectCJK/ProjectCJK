using Units.Buildings.Interfaces;
using Units.Buildings.Modules;
using Units.Buildings.Units.Abstract;
using UnityEngine;

namespace Units.Buildings.Units.InteractiveBuildings
{
    [RequireComponent(typeof(InteractionTrade))]
    public class Blender : InteractiveBuilding
    {
        private Grid _grid;
        private InteractionTrade _interactionTrade;

        private void Awake()
        {
            RegisterReference();
            RegisterEventListener();
        }

        public override void RegisterReference()
        {
            _grid = GetComponentInParent<Grid>();
            
            _interactionTrade = GetComponent<InteractionTrade>();
        }

        public override void Initialize()
        {
            RegisterEventListener();

            Vector3Int gridPosition = _grid.WorldToCell(transform.position);
            Debug.Log($"그리드 포지션: {gridPosition}");
        }

        private void RegisterEventListener()
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