namespace Units.Stages.Modules.BattleModules.Abstract
{
    public interface IBattleProperty
    {
        public int Damage { get; }
        public float AttackDelay { get; }
    }

    public abstract class BattleModule
    {
    }
}