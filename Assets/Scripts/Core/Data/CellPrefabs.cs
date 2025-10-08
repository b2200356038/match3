using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Data
{
    [CreateAssetMenu(fileName = "CellPrefabs", menuName = "Game/Prefabs")]
    public class CellPrefabs : ScriptableObject
    {
        [System.Serializable]
        public class CubePrefabMapping
        {
            public CubeType cubeType; 
            public GameObject prefab;
        }

        [System.Serializable]
        public class ObstaclePrefabMapping
        {
             public ObstacleType obstacleType;
             public GameObject prefab;
        }

        [System.Serializable]
        public class PowerUpPrefabMapping
        {
            public PowerUpType powerUpType;
            public GameObject prefab;
        }

        [Header("Cube Prefabs")] [SerializeField]
        private List<CubePrefabMapping> _cubePrefabs;

        [Header("Obstacle Prefabs")] [SerializeField]
        private List<ObstaclePrefabMapping> _obstaclePrefabs;

        [Header("PowerUp Prefabs")] [SerializeField]
        private List<PowerUpPrefabMapping> _powerUpPrefabs;
        private Dictionary<CubeType, GameObject> _cubePrefabDict;
        private Dictionary<ObstacleType, GameObject> _obstaclePrefabDict;
        private Dictionary<PowerUpType, GameObject> _powerUpPrefabDict;
        public void Initialize()
        {
            _cubePrefabDict = new Dictionary<CubeType, GameObject>();
            foreach (var mapping in _cubePrefabs)
                _cubePrefabDict[mapping.cubeType] = mapping.prefab;

            _obstaclePrefabDict = new Dictionary<ObstacleType, GameObject>();
            foreach (var mapping in _obstaclePrefabs)
                _obstaclePrefabDict[mapping.obstacleType] = mapping.prefab;

            _powerUpPrefabDict = new Dictionary<PowerUpType, GameObject>();
            foreach (var mapping in _powerUpPrefabs)
                _powerUpPrefabDict[mapping.powerUpType] = mapping.prefab;
        }
        public GameObject GetPrefab(CellData cellData)
        {
            if (cellData.IsEmpty) return null;

            return cellData.CellType switch
            {
                CellType.Cube => _cubePrefabDict.GetValueOrDefault(cellData.CubeType),
                CellType.Obstacle => _obstaclePrefabDict.GetValueOrDefault(cellData.ObstacleType),
                CellType.PowerUp => _powerUpPrefabDict.GetValueOrDefault(cellData.PowerUpType),
                _ => null 
            };
        }
    }
}