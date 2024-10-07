using System;
using Enums;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using UnityEngine;

namespace Units.Creatures.Abstract
{
    public abstract class BaseCreature : MonoBehaviour, IInitializable
    {
        public abstract void Initialize();
    }
}