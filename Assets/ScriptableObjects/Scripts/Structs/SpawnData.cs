using System;
using UnityEngine;

namespace ScriptableObjects.Scripts.Structs
{
    [Serializable]
    public struct SpawnData
    {
        public Vector3Int Position;
        public GameObject Prefab;
    }
}