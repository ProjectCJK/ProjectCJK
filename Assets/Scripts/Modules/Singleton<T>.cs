namespace Modules
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object _lock = new();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new T();
                    }
                }
                return _instance;
            }
        }
    }
}