using System;
using System.Collections.Generic;
using Buildings.Abstract;
using Buildings.Controllers;
using Creatures.Units;
using UnityEngine;

namespace Managers
{
    public class MainSceneManager : MonoBehaviour
    {
        public List<BaseBuilding> tempBuildingRef;

        private BuildingController _buildingController;

        private void Awake()
        {
            RegisterReference();
            InitializeReferences();
        }

        private void RegisterReference()
        {
            _buildingController = new BuildingController();
        }

        private void InitializeReferences()
        {
            _buildingController.Initialize(tempBuildingRef);
        }
    }
}