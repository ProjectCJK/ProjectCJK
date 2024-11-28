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

    public interface IRegisterReference<in T1, in T2, in T3, in T4, in T5>
    {
        public void RegisterReference(T1 instance1, T2 instance2, T3 instance3, T4 instance4, T5 instance5);
    }

    public interface IRegisterReference<in T1, in T2, in T3, in T4, in T5, in T6>
    {
        public void RegisterReference(T1 instance1, T2 instance2, T3 instance3, T4 instance4, T5 instance5, T6 instance6);
    }

    public interface IRegisterReference<in T1, in T2, in T3, in T4, in T5, in T6, in T7>
    {
        public void RegisterReference(T1 instance1, T2 instance2, T3 instance3, T4 instance4, T5 instance5, T6 instance6, T7 instance7);
    }
}