using Game.Core.Data;

namespace Game.Core
{
    public class BombCell : PowerUpCell
    {
        public override PowerUpType PowerUpType => PowerUpType.Bomb;

        public override void Activate(Board board)
        {
        }
    }
}