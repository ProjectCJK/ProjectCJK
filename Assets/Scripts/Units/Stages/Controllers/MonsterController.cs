using Units.Modules.FactoryModules.Units;

namespace Units.Stages.Controllers
{
    public interface IMonsterController
    {
        public IMonsterFactory MonsterFactory { get; }
    }
    
    public class MonsterController : IMonsterController
    {
        public IMonsterFactory MonsterFactory { get; }
        
        public MonsterController()
        {
            MonsterFactory = new MonsterFactory();
            MonsterFactory.CreateMonster();
        }
    }
}