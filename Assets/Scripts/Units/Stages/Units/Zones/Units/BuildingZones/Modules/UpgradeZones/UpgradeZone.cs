using System;
using AmplifyShaderEditor;
using UnityEngine;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Modules.UpgradeZones
{
    public interface IUpgradeZone
    {
        public event Action<bool> OnPlayerConnected;
    }
    
    public class UpgradeZone : MonoBehaviour, IUpgradeZone
    {
        public event Action<bool> OnPlayerConnected;

        private const string unitLayer = "Unit";
        private const string playerTag = "Player";

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(unitLayer) && other.gameObject.CompareTag(playerTag))
            {
                OnPlayerConnected?.Invoke(true);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(unitLayer) && other.gameObject.CompareTag(playerTag))
            {
                OnPlayerConnected?.Invoke(false);
            }
        }
    }
}