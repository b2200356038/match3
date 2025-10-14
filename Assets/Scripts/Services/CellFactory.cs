using Game.Core;
using Game.Core.Data;
using UnityEngine;

namespace Game.Services
{
    public class CellFactory
    {
        private readonly CellConfig _cellConfig;

        public CellFactory(CellConfig cellConfig)
        {
            _cellConfig = cellConfig;
            _cellConfig.Initialize();
        }

        public CubeCell CreateCube(CubeType cubeType, Vector2Int position)
        {
            var prefab = _cellConfig.GetCubePrefab(cubeType);
            if (prefab == null) return null;

            var cell = Object.Instantiate(prefab).GetComponent<CubeCell>();
            cell.Init(position);
            cell.SetColor(cubeType);
            return cell;
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
    }
}