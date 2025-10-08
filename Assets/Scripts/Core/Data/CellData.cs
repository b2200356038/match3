using UnityEngine;

namespace Game.Core.Data
{
    public struct CellData
    {
        public CubeType? CubeType { get; private set; }
        public ObstacleType? ObstacleType { get; private set; }
        public PowerUpType? PowerUpType { get; private set; }
        public Vector2Int Position { get; private set; }
        public CellState State { get; private set; }
        
        public CellData(CubeType cubeType, Vector2Int position, CellState state = CellState.Idle)
        {
            CubeType = cubeType;
            ObstacleType = null;
            PowerUpType = null;
            Position = position;
            State = state;
        }
        
        public CellData(ObstacleType obstacleType, Vector2Int position, CellState state = CellState.Idle)
        {
            CubeType = null;
            ObstacleType = obstacleType;
            PowerUpType = null;
            Position = position;
            State = state;
        }
        
        public CellData(PowerUpType powerUpType, Vector2Int position, CellState state = CellState.Idle)
        {
            CubeType = null;
            ObstacleType = null;
            PowerUpType = powerUpType;
            Position = position;
            State = state;
        }
        
        public bool IsEmpty => !CubeType.HasValue && !ObstacleType.HasValue && !PowerUpType.HasValue;
        public bool IsCube => CubeType.HasValue;
        public bool IsObstacle => ObstacleType.HasValue;
        public bool IsPowerUp => PowerUpType.HasValue;
        public bool IsPlayable => IsCube || IsPowerUp;
        
        public bool IsFallable => IsCube || IsPowerUp;
        public bool CanClick => State == CellState.Idle && IsPlayable;
        public bool CanMove => State != CellState.Disabled;
        public CellData WithState(CellState newState)
        {
            if (IsCube) return new CellData(CubeType.Value, Position, newState);
            if (IsObstacle) return new CellData(ObstacleType.Value, Position, newState);
            if (IsPowerUp) return new CellData(PowerUpType.Value, Position, newState);
            return CreateEmpty(Position);
        }

        public CellData WithCubeType(CubeType newCubeType)
        {
            return new CellData(newCubeType, Position, State);
        }

        public static CellData CreateEmpty(Vector2Int position)
        {
            var slot = new CellData
            {
                CubeType = null,
                ObstacleType = null,
                PowerUpType = null,
                Position = position,
                State = CellState.Idle
            };
            return slot;
        }
    }
}