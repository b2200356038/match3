namespace Game.Core.Data
{
    public enum CubeType
    {
        Empty,
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