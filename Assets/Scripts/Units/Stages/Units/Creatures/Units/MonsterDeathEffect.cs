using System;
using System.Collections;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using IInitializable = Unity.VisualScripting.IInitializable;
using IPoolable = Modules.DesignPatterns.ObjectPools.IPoolable;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IMonsterDeathEffect : IPoolable, IRegisterReference<Action>, IInitializable<Vector3>
    {
    }
    
    public class MonsterDeathEffect : MonoBehaviour, IMonsterDeathEffect
    {
        private event Action OnParticleEnded;

        private ParticleSystem _particleSystem;
        private readonly WaitForSeconds _waitForSeconds = new(3f);
        
        public void RegisterReference(Action onParticleEnded)
        {
            OnParticleEnded = onParticleEnded;
        }
        
        public void Initialize(Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive(true);

            // 3초 후에 Invoke 실행
            StartCoroutine(DelayedParticleEnd());
        }

        private IEnumerator DelayedParticleEnd()
        {
            _particleSystem.Play();
            yield return _waitForSeconds;
            OnParticleEnded?.Invoke();
            _particleSystem.Stop();
            ReturnToPool(); // 파티클을 풀로 반환
        }

        public void Create()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            gameObject.SetActive(false);
        }

        public void GetFromPool()
        {
            gameObject.SetActive(false);
        }

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
    }
}