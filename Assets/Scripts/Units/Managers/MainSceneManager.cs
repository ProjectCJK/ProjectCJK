using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Interfaces;
using Units.Buildings.Abstract;
using Units.Buildings.Controllers;
using Units.Creatures.Abstract;
using Units.Creatures.Controllers;
using UnityEngine;

namespace Units.Managers
{
    public class MainSceneManager : MonoBehaviour, IReferenceRegisterable
    {
        [SerializeField] private BaseCreature test;
        
        public List<BaseCreature> units;
        public List<BaseBuilding> tempBuildingRef;

        private CreatureController _creatureController;
        private BuildingController _buildingController;

        private void Awake()
        {
            RegisterReference();
            InitializeReferences();
        }

        public void RegisterReference()
        {
            _creatureController = new CreatureController();
            _buildingController = new BuildingController();
        }

        private void InitializeReferences()
        {
            _creatureController.RegisterReference(test);
            _buildingController.Initialize(tempBuildingRef);
        }

        public static IEnumerator Timer(float seconds, Action<bool> callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke(true);
        }
    }
}