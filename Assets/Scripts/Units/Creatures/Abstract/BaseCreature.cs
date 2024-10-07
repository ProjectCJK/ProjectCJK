using System;
using UnityEngine;

namespace Units.Creatures.Abstract
{
    public abstract class BaseCreature : MonoBehaviour
    {
        public event Action<int> OnConnect;
        public event Action<int> OnDisconnect;
    }
}