using Units.Stages.Modules.FSMModules.Units.Creature;

namespace Units.Stages.Modules.FSMModules.Units.Monsters
{
    public class MonsterStateMachine : CreatureStateMachine
    {
        public MonsterIdleState MonsterIdleState { get; private set; }
        public MonsterRunState MonsterRunState { get; private set; }
        
        public MonsterStateMachine(Stages.Units.Creatures.Abstract.Creature creature) : base(creature)
        {
            MonsterIdleState = new MonsterIdleState(this);
            MonsterRunState = new MonsterRunState(this);
        }
    }
}