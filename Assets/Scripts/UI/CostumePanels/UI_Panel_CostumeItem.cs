using Managers;
using Modules.DesignPatterns.ObjectPools;
using UnityEngine;

namespace UI.CostumePanels
{
    public class UI_Panel_CostumeItem : MonoBehaviour, IPoolable
    {
        public void Initialize(CostumeItemData costumeItem)
        {
            
        }
        
        public void Create()
        {
            gameObject.SetActive(false);
        }

        public void GetFromPool()
        {
            gameObject.SetActive(true);
        }

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
    }
}