using UnityEngine;

namespace Game.Core.Data
{
    public struct CellData
    {
        public CellType CellType { get; private set; }
        public CubeType CubeType { get; private set; }
        public ObstacleType ObstacleType { get; private set; }
        public PowerUpType PowerUpType { get; private set; }
        public Vector2Int Position { get; private set; }
        public CellState State { get; private set; }

        private CellData(Vector2Int position, CellState state = CellState.Idle)
        {
            CellType = CellType.Empty;
            CubeType = default;
            ObstacleType = default;
            PowerUpType = default;
            Position = position;
            State = state;
        }

        public static CellData CreateCube(CubeType cubeType, Vector2Int position, CellState state = CellState.Idle)
        {
            if (cubeType == CubeType.Empty)
            {
                return CreateEmpty(position, state);
            }

            return new CellData(position, state) { CellType = CellType.Cube, CubeType = cubeType };
        }

        public static CellData CreateObstacle(ObstacleType obstacleType, Vector2Int position,
            CellState state = CellState.Idle)
        {
            return new CellData(position, state) { CellType = CellType.Obstacle, ObstacleType = obstacleType };
        }

        public static CellData CreatePowerUp(PowerUpType powerUpType, Vector2Int position,
            CellState state = CellState.Idle)
        {
            return new CellData(position, state) { CellType = CellType.PowerUp, PowerUpType = powerUpType };
        }

        public static CellData CreateEmpty(Vector2Int position, CellState state = CellState.Idle)
        {
            return new CellData(position, state);
        }

        public bool IsEmpty => CellType == CellType.Empty;
        public bool IsCube => CellType == CellType.Cube;
        public bool IsObstacle => CellType == CellType.Obstacle;
        public bool IsPowerUp => CellType == CellType.PowerUp;
        public bool IsPlayable => IsCube || IsPowerUp;
        public bool IsFallable => IsCube || IsPowerUp;
        public bool CanClick => State == CellState.Idle && IsPlayable;
        public bool CanMove => State != CellState.Disabled;

        public CellData WithState(CellState newState)
        {
            var newData = this;
            newData.State = newState;
            return newData;
        }
    }
}