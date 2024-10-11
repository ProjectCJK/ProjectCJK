
using System;
using Modules.DesignPatterns.ObjectPools;
using UnityEngine;

namespace Units.Games.Items.Units
{
    public interface IItem
    {
        
    }
    
    public abstract class Item : MonoBehaviour, IItem, IPoolable
    {
        public event Action OnCreateEvent;
        public event Action OnGetFromPoolEvent;
        public event Action OnReturnToPoolEvent;
        
        public void OnCreate()
        {
            
        }

        public void OnGetFromPool()
        {
            
        }

        public void OnReturnToPool()
        {
            
        }

        public void Reset()
        {
            
        }
    }
}