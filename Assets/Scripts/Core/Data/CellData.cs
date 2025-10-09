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
        
        public int Health { get; private set; }
        public bool CanFall { get; private set; }

        private CellData(Vector2Int position, CellState state = CellState.Idle)
        {
            CellType = CellType.Empty;
            CubeType = default;
            ObstacleType = default;
            PowerUpType = default;
            Position = position;
            State = state;
            Health = 0;
            CanFall = false;
        }
        
        public static CellData CreateEmpty(Vector2Int position, CellState state = CellState.Idle)
        {
            return new CellData(position, state);
        }
        
        public static CellData CreateCube(CubeType cubeType, Vector2Int position, int health = 1, bool canFall = true, CellState state = CellState.Idle)
        {
            return new CellData(position, state) 
            { 
                CellType = CellType.Cube, 
                CubeType = cubeType,
                Health = health,
                CanFall = canFall
            };
        }

        public static CellData CreateObstacle(ObstacleType obstacleType, Vector2Int position, int health = 1, bool canFall = false, CellState state = CellState.Idle)
        {
            return new CellData(position, state) 
            { 
                CellType = CellType.Obstacle, 
                ObstacleType = obstacleType,
                Health = health,
                CanFall = canFall
            };
        }

        public static CellData CreatePowerUp(PowerUpType powerUpType, Vector2Int position, int health = 1,
            bool canFall = true, CellState state = CellState.Idle)
        {
            return new CellData(position, state)
            {
                CellType = CellType.PowerUp,
                PowerUpType = powerUpType,
                Health = health,
                CanFall = canFall
            };
        }
        
        public bool IsEmpty => CellType == CellType.Empty;
        public bool IsCube => CellType == CellType.Cube;
        public bool IsObstacle => CellType == CellType.Obstacle;
        public bool IsPowerUp => CellType == CellType.PowerUp;
        public bool IsFallable => CanFall && State != CellState.Disabled;
        public bool CanClick => State == CellState.Idle && (IsCube || IsPowerUp);
        public bool CanMatch => IsCube && State == CellState.Idle;
        public bool IsBreakable => Health > 0;
        public CellData TakeDamage(int damage = 1)
        {
            var newData = this;
            newData.Health = Mathf.Max(0, Health - damage);
            return newData;
        }
        
        public CellData ToggleCanFall()
        {
            var newData = this;
            newData.CanFall = !CanFall;
            return newData;
        }
        public CellData WithState(CellState newState)
        {
            var newData = this;
            newData.State = newState;
            return newData;
        }
    }
}