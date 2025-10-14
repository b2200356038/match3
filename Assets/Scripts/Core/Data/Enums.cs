namespace Game.Core.Data
{
    public enum CellState
    {
        Idle,
        Falling,
        Matched,
        Locked
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
}