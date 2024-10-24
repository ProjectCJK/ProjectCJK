using Units.Modules.MovementModules.Abstract;

namespace Units.Modules.MovementModules.Units
{
    public interface IMonsterMovementModule
    {
        
    }
    
    public class MonsterMovementModule : IMonsterMovementModule
    {
        public MonsterMovementModule(IMovementProperty monsterStatModule)
        {
            
        }
    }
}