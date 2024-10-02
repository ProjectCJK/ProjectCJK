using System;
using Buildings.Abstract;
using Buildings.Interfaces;
using Buildings.Units.Abstract;
using UnityEngine;

namespace Buildings.Units.InteractiveBuildings
{
    public class TestBlender : InteractiveBuilding, ISendObject, IReceiveObject
    {
        private Grid _grid;
        
        public override void Initialize()
        {
            interactionTrade.OnSendObject += SendObject;
            interactionTrade.OnReceiveObject += ReceiveObject;
        }

        private void Awake()
        {
            _grid = GetComponentInParent<Grid>();
            Vector3Int gridPosition = _grid.WorldToCell(transform.position);
            Debug.Log($"그리드 포지션: {gridPosition}");
        }

        public void SendObject()
        {
            
        }

        public void ReceiveObject()
        {
            
        }
    }
}