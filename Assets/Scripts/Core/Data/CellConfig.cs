using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Data
{
    [CreateAssetMenu(fileName = "CellConfig", menuName = "Game/CellConfig")]
    public class CellConfig : ScriptableObject
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

        [Header("Cube Prefabs")]
        [SerializeField] private List<CubePrefabMapping> cubePrefabs;

        [Header("Obstacle Prefabs")]
        [SerializeField] private List<ObstaclePrefabMapping> obstaclePrefabs;

        [Header("PowerUp Prefabs")]
        [SerializeField] private List<PowerUpPrefabMapping> powerUpPrefabs;

        private Dictionary<CubeType, GameObject> _cubeDict;
        private Dictionary<ObstacleType, GameObject> _obstacleDict;
        private Dictionary<PowerUpType, GameObject> _powerUpDict;

        public void Initialize()
        {
            _cubeDict = new Dictionary<CubeType, GameObject>();
            foreach (var mapping in cubePrefabs)
                _cubeDict[mapping.cubeType] = mapping.prefab;

            _obstacleDict = new Dictionary<ObstacleType, GameObject>();
            foreach (var mapping in obstaclePrefabs)
                _obstacleDict[mapping.obstacleType] = mapping.prefab;

            _powerUpDict = new Dictionary<PowerUpType, GameObject>();
            foreach (var mapping in powerUpPrefabs)
                _powerUpDict[mapping.powerUpType] = mapping.prefab;
        }

        public GameObject GetCubePrefab(CubeType type)
        {
            return _cubeDict.GetValueOrDefault(type);
        }

        public GameObject GetObstaclePrefab(ObstacleType type)
        {
            return _obstacleDict.GetValueOrDefault(type);
        }

        public GameObject GetPowerUpPrefab(PowerUpType type)
        {
            return _powerUpDict.GetValueOrDefault(type);
        }

        public List<CubeType> GetAllCubeTypes()
        {
            List<CubeType> types = new List<CubeType>();
            foreach (var mapping in cubePrefabs)
            {
                if (mapping.prefab != null)
                    types.Add(mapping.cubeType);
            }
            return types;
        }
    }
}