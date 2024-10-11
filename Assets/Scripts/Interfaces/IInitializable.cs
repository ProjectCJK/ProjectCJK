namespace Interfaces
{
    public interface IInitializable
    {
        public void Initialize();
    }
    
    public interface IInitializable<in T>
    {
        public void Initialize(T _inputItemKey);
    }
    
    public interface IInitializable<in T1, in T2>
    {
        public void Initialize(T1 instance1, T2 instance2);
    }
}