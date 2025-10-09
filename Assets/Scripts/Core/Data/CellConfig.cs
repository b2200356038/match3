using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Data
{
    [CreateAssetMenu(fileName = "CellConfig", menuName = "Game/Config")]
    public class CellConfig : ScriptableObject
    {
        [System.Serializable]
        public class CubePrefabMapping
        {
            public CubeType cubeType; 
            public GameObject prefab;
            [Header("Properties")]
            public int health = 1;
            public bool canFall = true;
        }

        [System.Serializable]
        public class ObstaclePrefabMapping
        {
            public ObstacleType obstacleType;
            public GameObject prefab;
            [Header("Properties")]
            public int health = 1;
            public bool canFall = false;
        }

        [System.Serializable]
        public class PowerUpPrefabMapping
        {
            public PowerUpType powerUpType;
            public GameObject prefab;
            [Header("Properties")]
            public int health = 1;
            public bool canFall = true;
        }

        [Header("Cube Prefabs")] [SerializeField]
        private List<CubePrefabMapping> _cubePrefabs;

        [Header("Obstacle Prefabs")] [SerializeField]
        private List<ObstaclePrefabMapping> _obstaclePrefabs;

        [Header("PowerUp Prefabs")] [SerializeField]
        private List<PowerUpPrefabMapping> _powerUpPrefabs;
        
        private Dictionary<CubeType, CubePrefabMapping> _cubeDict;
        private Dictionary<ObstacleType, ObstaclePrefabMapping> _obstacleDict;
        private Dictionary<PowerUpType, PowerUpPrefabMapping> _powerUpDict;
        
        public void Initialize()
        {
            _cubeDict = new Dictionary<CubeType, CubePrefabMapping>();
            foreach (var mapping in _cubePrefabs)
                _cubeDict[mapping.cubeType] = mapping;

            _obstacleDict = new Dictionary<ObstacleType, ObstaclePrefabMapping>();
            foreach (var mapping in _obstaclePrefabs)
                _obstacleDict[mapping.obstacleType] = mapping;

            _powerUpDict = new Dictionary<PowerUpType, PowerUpPrefabMapping>();
            foreach (var mapping in _powerUpPrefabs)
                _powerUpDict[mapping.powerUpType] = mapping;
        }
        
        public GameObject GetPrefab(CellData cellData)
        {
            if (cellData.IsEmpty) return null;

            return cellData.CellType switch
            {
                CellType.Cube => _cubeDict.GetValueOrDefault(cellData.CubeType)?.prefab,
                CellType.Obstacle => _obstacleDict.GetValueOrDefault(cellData.ObstacleType)?.prefab,
                CellType.PowerUp => _powerUpDict.GetValueOrDefault(cellData.PowerUpType)?.prefab,
                _ => null 
            };
        }
        
        public CubePrefabMapping GetCubeMapping(CubeType type)
        {
            return _cubeDict.GetValueOrDefault(type);
        }
        
        public ObstaclePrefabMapping GetObstacleMapping(ObstacleType type)
        {
            return _obstacleDict.GetValueOrDefault(type);
        }
        
        public PowerUpPrefabMapping GetPowerUpMapping(PowerUpType type)
        {
            return _powerUpDict.GetValueOrDefault(type);
        }
    }
}