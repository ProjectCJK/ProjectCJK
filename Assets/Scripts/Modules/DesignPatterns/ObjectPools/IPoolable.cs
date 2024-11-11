namespace Modules.DesignPatterns.ObjectPools
{
    /// <summary>
    ///     풀링된 오브젝트가 구현해야 할 인터페이스.
    ///     오브젝트의 생명주기를 관리합니다.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        ///     오브젝트가 생성될 때 호출되는 메서드.
        /// </summary>
        public void Create();

        /// <summary>
        ///     오브젝트가 풀에서 가져올 때 호출되는 메서드.
        /// </summary>
        public void GetFromPool();

        /// <summary>
        ///     오브젝트가 풀로 반환될 때 호출되는 메서드.
        /// </summary>
        public void ReturnToPool();
    }
}