namespace Interfaces
{
    public interface IReferenceRegisterable
    {
        public void RegisterReference();
    }
    
    public interface IReferenceRegisterable<in T>
    {
        public void RegisterReference(T instance);
    }
    
    public interface IReferenceRegisterable<in T1, in T2>
    {
        public void RegisterReference(T1 instance1, T2 instance2);
    }
    
    public interface IReferenceRegisterable<in T1, in T2, in T3>
    {
        public void RegisterReference(T1 instance1, T2 instance2, T3 instance3);
    }
}