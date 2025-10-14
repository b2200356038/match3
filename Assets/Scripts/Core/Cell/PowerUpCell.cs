using Game.Core.Data;

namespace Game.Core
{
    public abstract class PowerUpCell : Cell
    {
        public abstract PowerUpType PowerUpType { get; }
        
        public virtual void Activate(Board board)
        {
        }
    }
}