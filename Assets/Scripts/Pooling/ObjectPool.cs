using System.Collections.Generic;
using UnityEngine;

namespace Game.Infrastructure
{
    public class ObjectPool<T> where T : Component
    {
        private Queue<T> _pool;
        private T _prefab;
        private Transform _parent;
        
        public int TotalCount { get; private set; }
        public int ActiveCount => TotalCount - _pool.Count;
        public int InactiveCount => _pool.Count;
        
        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            if (prefab == null)
            {
                return;
            }
            
            _prefab = prefab;
            _parent = parent;
            _pool = new Queue<T>(initialSize);
            TotalCount = 0;
            
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }
        
        private T CreateNewObject()
        {
            T obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
            TotalCount++;
            return obj;
        }
        
        public T Get()
        {
            T obj;
            
            if (_pool.Count == 0)
            {
                obj = CreateNewObject();
            }
            else
            {
                obj = _pool.Dequeue();
            }
            
            obj.gameObject.SetActive(true);
            return obj;
        }
        
        public void Return(T obj)
        {
            if (obj == null)
            {
                return;
            }
            
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
        
        public void Clear()
        {
            while (_pool.Count > 0)
            {
                var obj = _pool.Dequeue();
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }
            
            _pool.Clear();
            TotalCount = 0;
        }
    }
}