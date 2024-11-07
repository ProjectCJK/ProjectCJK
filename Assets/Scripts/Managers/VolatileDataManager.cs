using System.Collections.Generic;
using Interfaces;
using Units.Stages.Units.Items.Enums;
using Unity.VisualScripting;

namespace Managers
{
    public class VolatileDataManager : Modules.DesignPatterns.Singletons.Singleton<VolatileDataManager>, IRegisterReference
    {
        public Dictionary<EMaterialType, EStageMaterialType> MaterialMappings = new();
        
        public int CurrentStageLevel;
        
        public void RegisterReference()
        {
         
        }
    }
}