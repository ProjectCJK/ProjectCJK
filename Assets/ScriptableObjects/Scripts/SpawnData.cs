using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects.Scripts
{
    [Serializable]
    public struct SpawnData
    {
        public Vector3Int Position;
        public GameObject Prefab;
    }
}