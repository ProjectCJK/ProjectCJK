namespace Interfaces
{
    public interface IRegisterReference
    {
        public void RegisterReference();
    }
    
    public interface IRegisterReference<in T>
    {
        public void RegisterReference(T instance);
    }
    
    public interface IRegisterReference<in T1, in T2>
    {
        public void RegisterReference(T1 instance1, T2 instance2);
    }
    
    public interface IRegisterReference<in T1, in T2, in T3>
    {
        public void RegisterReference(T1 instance1, T2 instance2, T3 instance3);
    }
    
    public interface IRegisterReference<in T1, in T2, in T3, in T4>
    {
        public void RegisterReference(T1 instance1, T2 instance2, T3 instance3, T4 instance4);
    }
}