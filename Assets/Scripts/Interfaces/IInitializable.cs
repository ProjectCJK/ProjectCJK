namespace Interfaces
{
    public interface IInitializable
    {
        public void Initialize();
    }

    public interface IInitializable<in T1>
    {
        public void Initialize(T1 instance1);
    }

    public interface IInitializable<in T1, in T2>
    {
        public void Initialize(T1 instance1, T2 instance2);
    }

    public interface IInitializable<in T1, in T2, in T3>
    {
        public void Initialize(T1 instance1, T2 instance2, T3 instance3);
    }

    public interface IInitializable<in T1, in T2, in T3, in T4>
    {
        public void Initialize(T1 instance1, T2 instance2, T3 instance3, T4 instance4);
    }
    
    public interface IInitializable<in T1, in T2, in T3, in T4, in T5>
    {
        public void Initialize(T1 instance1, T2 instance2, T3 instance3, T4 instance4, T5 instance5);
    }
}