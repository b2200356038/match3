using Game.Core;
using Game.Core.Data;
using Game.Infrastructure;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Services
{
    public class CellFactory
    {
        private readonly CellConfig _cellConfig;
        
        private Dictionary<CubeType, ObjectPool<CubeCell>> _cubePools;
        private Dictionary<ObstacleType, ObjectPool<ObstacleCell>> _obstaclePools;
        private Dictionary<PowerUpType, ObjectPool<PowerUpCell>> _powerUpPools;
        //add config
        private const int INITIAL_POOL_SIZE = 20;
        private Transform _poolParent;

        public CellFactory(CellConfig cellConfig)
        {
            _cellConfig = cellConfig;
            _cellConfig.Initialize();
            var poolParentObj = new GameObject("CellPools");
            Object.DontDestroyOnLoad(poolParentObj);
            _poolParent = poolParentObj.transform;
            
            InitializePools();
        }
        
        private void InitializePools()
        {
            _cubePools = new Dictionary<CubeType, ObjectPool<CubeCell>>();
            _obstaclePools = new Dictionary<ObstacleType, ObjectPool<ObstacleCell>>();
            _powerUpPools = new Dictionary<PowerUpType, ObjectPool<PowerUpCell>>();
            var cubeTypes = _cellConfig.GetAllCubeTypes();
            foreach (var cubeType in cubeTypes)
            {
                var prefab = _cellConfig.GetCubePrefab(cubeType);
                if (prefab != null)
                {
                    var cubeComponent = prefab.GetComponent<CubeCell>();
                    if (cubeComponent != null)
                    {
                        var pool = new ObjectPool<CubeCell>(cubeComponent, INITIAL_POOL_SIZE, _poolParent);
                        _cubePools.Add(cubeType, pool);
                        Debug.Log($"Created pool for {cubeType} with size {INITIAL_POOL_SIZE}");
                    }
                }
            }
        }
        
        public CubeCell CreateCube(CubeType cubeType, Vector2Int position)
        {
            if (!_cubePools.ContainsKey(cubeType))
            {
                return null;
            }
            
            var cell = _cubePools[cubeType].Get();
            cell.Init(position);
            cell.SetColor(cubeType);
            return cell;
        }
        
        public void ReturnCube(CubeCell cell)
        {
            if (cell == null) return;
            
            if (_cubePools.ContainsKey(cell.ColorType))
            {
                _cubePools[cell.ColorType].Return(cell);
            }
            else
            {
                Object.Destroy(cell.gameObject);
            }
        }

        public ObstacleCell CreateObstacle(ObstacleType obstacleType, Vector2Int position)
        {
            var prefab = _cellConfig.GetObstaclePrefab(obstacleType);
            if (prefab == null) return null;

            var cell = Object.Instantiate(prefab).GetComponent<ObstacleCell>();
            cell.Init(position);
            return cell;
        }

        public PowerUpCell CreatePowerUp(PowerUpType powerUpType, Vector2Int position)
        {
            var prefab = _cellConfig.GetPowerUpPrefab(powerUpType);
            if (prefab == null) return null;

            var cell = Object.Instantiate(prefab).GetComponent<PowerUpCell>();
            cell.Init(position);
            return cell;
        }

        public CubeCell CreateRandomCube(Vector2Int position)
        {
            var allCubes = _cellConfig.GetAllCubeTypes();
            if (allCubes == null || allCubes.Count == 0)
                return null;

            var randomIndex = Random.Range(0, allCubes.Count);
            var randomCubeType = allCubes[randomIndex];

            return CreateCube(randomCubeType, position);
        }
        
        public void ClearAllPools()
        {
            foreach (var pool in _cubePools.Values)
            {
                pool.Clear();
            }
            
            _cubePools.Clear();
            
            if (_poolParent != null)
            {
                Object.Destroy(_poolParent.gameObject);
            }
        }
    }
}