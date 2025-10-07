namespace Game.Core.Data
{
    public enum CellType
    {
        Empty,
        Red,
        Green,
        Blue,
        Yellow
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