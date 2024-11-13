using UnityEngine;

namespace Modules
{
    public class CompositeColliderBaker : MonoBehaviour
    {
        private void OnEnable()
        {
            var compositeCollider = GetComponent<CompositeCollider2D>();
            
            if (compositeCollider != null)
            {
                compositeCollider.GenerateGeometry();
            }
        }
    }
}