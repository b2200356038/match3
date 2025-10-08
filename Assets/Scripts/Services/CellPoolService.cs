using System.Collections.Generic;
using Game.Core.Data;
using UnityEngine;

namespace Game.Services
{
    public class CellPoolService
    {
        private readonly CellPrefabs _cellPrefabs;
        private readonly Dictionary<GameObject, Queue<PoolableObject>> _pool;
        private readonly Transform _poolParent; 

        public CellPoolService(CellPrefabs cellPrefabs)
        {
            _cellPrefabs = cellPrefabs;
            _cellPrefabs.Initialize();
            _poolParent = new GameObject("CellPool").transform;
            _pool = new Dictionary<GameObject, Queue<PoolableObject>>(); // ← DÜZELTME: Dictionary'yi initialize et
        }

        public PoolableObject Get(CellData cellData)
        {
            if (cellData.IsEmpty) return null;

            GameObject prefab = _cellPrefabs.GetPrefab(cellData);
            if (prefab == null)
            {
                Debug.LogError($"Prefab for {cellData.CellType} not found!");
                return null;
            }
            
            Debug.Log(cellData.CellType);

            if (_pool.TryGetValue(prefab, out var queue) && queue.Count > 0)
            {
                PoolableObject obj = queue.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }
            return CreateNewObject(prefab);
        }

        private PoolableObject CreateNewObject(GameObject prefab)
        {
            GameObject newObj = Object.Instantiate(prefab);
            PoolableObject poolable = newObj.GetComponent<PoolableObject>();
            if (poolable == null)
            {
                poolable = newObj.AddComponent<PoolableObject>();
            }
            poolable.SetReturnAction(obj => Return(prefab, obj));
            
            return poolable;
        }

        private void Return(GameObject prefab, PoolableObject obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_poolParent); 
            if (!_pool.ContainsKey(prefab))
            {
                _pool[prefab] = new Queue<PoolableObject>();
            }
            _pool[prefab].Enqueue(obj);
        }
    }
}