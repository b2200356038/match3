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

        public CellData CreateCube(CubeType cubeType, Vector2Int position)
        {
            var mapping = _cellConfig.GetCubeMapping(cubeType);
            if (mapping == null)
            {
                return CellData.CreateEmpty(position);
            }

            return CellData.CreateCube(
                cubeType, 
                position, 
                mapping.health, 
                mapping.canFall
            );
        }

        public CellData CreateObstacle(ObstacleType obstacleType, Vector2Int position)
        {
            var mapping = _cellConfig.GetObstacleMapping(obstacleType);
            
            return CellData.CreateObstacle(
                obstacleType, 
                position, 
                mapping.health, 
                mapping.canFall
            );
        }

        public CellData CreatePowerUp(PowerUpType powerUpType, Vector2Int position)
        {
            var mapping = _cellConfig.GetPowerUpMapping(powerUpType);
            return CellData.CreatePowerUp(
                powerUpType, 
                position, 
                mapping.health, 
                mapping.canFall
            );
        }

        public CellData CreateRandomCube(Vector2Int position, int colorCount)
        {
            int random = Random.Range(0, colorCount);
            CubeType cubeType = random switch
            {
                0 => CubeType.Red,
                1 => CubeType.Blue,
                2 => CubeType.Green,
                3 => CubeType.Yellow,
                _ => CubeType.Red
            };

            return CreateCube(cubeType, position);
        }

        public CellData CreateEmpty(Vector2Int position)
        {
            return CellData.CreateEmpty(position);
        }
    }
}