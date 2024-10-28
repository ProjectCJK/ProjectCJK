using System;
using UnityEngine;

namespace Modules.DesignPatterns.FSMs.Modules
{
    [Serializable]
    public static class AnimationParameterData
    {
        public static int IdleParameterHash { get; private set; } = Animator.StringToHash("Idle");
        public static int RunParameterHash { get; private set; } = Animator.StringToHash("Run");
        public static int HitParameterHash { get; private set; } = Animator.StringToHash("Hit");
        public static int DieParameterHash { get; private set; } = Animator.StringToHash("Die");
    }
}