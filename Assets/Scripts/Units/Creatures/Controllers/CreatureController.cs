using System;
using System.Collections.Generic;
using Interfaces;
using Units.Creatures.Abstract;
using Unity.VisualScripting;
using UnityEngine;
using IInitializable = Interfaces.IInitializable;

namespace Units.Creatures.Controllers
{
    public class CreatureController : MonoBehaviour, IReferenceRegisterable<BaseCreature>, IInitializable
    {
        public Action<GameObject, bool> OnConnectInteraction;
        
        private BaseCreature _player;
        private readonly Dictionary<int, BaseCreature> _creatures = new();
        
        public void RegisterReference(BaseCreature player)
        {
            _player = player;
        }
        
        public void Initialize()
        {
            InstantiatePlayer();
        }
        
        private void InstantiatePlayer()
        {
            BaseCreature player = Instantiate(_player);
            
            _creatures.TryAdd(player.GetInstanceID(), player);
        }
    }
}