using Game.Core.Data;

namespace Game.Core
{
    public class RocketCell : PowerUpCell
    {
        public bool IsVertical;

        public override PowerUpType PowerUpType => 
            IsVertical ? PowerUpType.ColumnRocket : PowerUpType.RowRocket;

        public override void Activate(Board board)
        {
        }
    }
}