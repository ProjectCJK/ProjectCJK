using System;
using System.Collections.Generic;
using Enums;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Buildings.Controllers;
using Units.Creatures.Abstract;
using UnityEngine;

namespace Units.Creatures.Controllers
{
    public interface ICreatureController : IReferenceRegisterable<List<EMaterialType>, IBuildingController>, IInitializable
    {
        public void SpawnPlayer(SpawnData playerSpawnData, Joystick joystick);
        public Transform GetPlayerTransform();
    }
    
    public class CreatureController : ICreatureController
    {
        public Action<GameObject, bool> OnConnect;
        
        private readonly IPlayerFactory _playerFactory;
        private readonly Dictionary<int, BaseCreature> _creatures = new();
        
        private BaseCreature _player;
        private List<EMaterialType> _materialType;
        private IBuildingController _buildingController;
        
        public CreatureController(IPlayerFactory playerFactory)
        {
            _playerFactory = playerFactory;
        }

        public void RegisterReference(List<EMaterialType> materialType, IBuildingController buildingController)
        {
            _materialType = materialType;
            _buildingController = buildingController;
        }

        public void Initialize()
        {
            _player.Initialize();
        }

        public void SpawnPlayer(SpawnData playerSpawnData, Joystick joystick)
        {
            _player = _playerFactory.CreatePlayer(playerSpawnData, joystick, HandleBuildingInteraction);
            
            RegisterInstanceID(_player);
        }

        public Transform GetPlayerTransform()
        {
            return _player.transform;
        }

        private bool RegisterInstanceID(BaseCreature creature)
        {
            return _creatures.TryAdd(creature.gameObject.GetInstanceID(), creature);
        }
        
        private void HandleBuildingInteraction(int? buildingInstanceID, bool tryConnect)
        {
            switch (tryConnect)
            {
                case true:
                    Debug.Log($"InstanceID : {buildingInstanceID} 빌딩과 Interaction Connect 시행");
                    if (buildingInstanceID.HasValue)
                    {
                        _buildingController.AddToBuildingQueue(buildingInstanceID.Value, _player);  // 빌딩 큐에 플레이어 추가
                    }
                    break;
                case false:
                    Debug.Log($"InstanceID : {buildingInstanceID} 빌딩과 Interaction Disconnect 시행");
                    if (buildingInstanceID.HasValue)
                    {
                        _buildingController.RemoveFromBuildingQueue(buildingInstanceID.Value, _player);  // 빌딩 큐에 플레이어 추가
                    }
                    break;
            }
        }
    }
}