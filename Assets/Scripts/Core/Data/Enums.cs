namespace Game.Core.Data
{

    public enum CellType
    {
        Empty,
        Cube,
        Obstacle,
        PowerUp
    }
    
    public enum CubeType
    {
        Red,
        Green,
        Blue,
        Yellow
    }
    
    
    public enum ObstacleType
    {
        Rock,
        Ice,
        Chain
    }
    
    public enum PowerUpType
    {
        RowRocket,
        ColumnRocket,
        Bomb
    }
    
    public enum CellState
    {
        Idle,
        Moving,
        Matched,
        Disabled
    }

    public enum GameState
    {
        Initializing,
        Playing,
        Processing,
        Win,
        Lose
    }
}