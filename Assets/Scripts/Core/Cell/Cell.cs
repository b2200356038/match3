using UnityEngine;
using Game.Core.Data;

namespace Game.Core
{
    public abstract class Cell : MonoBehaviour
    {
        public Vector2Int Position { get; protected set; }
        public CellState State { get; set; } = CellState.Idle;

        public virtual bool CanMove { get; protected set; } = true;
        public virtual bool CanFall { get; protected set; } = true;
        public virtual bool CanMatch { get; protected set; } = true;
        public float FallTime { get; protected set; }
        public float SpeedMultiplier { get; set; } = 1.0f;

        public virtual void Init(Vector2Int position)
        {
            Position = position;
            State = CellState.Idle;
        }
        public virtual void MoveTo(Vector2Int newPosition)
        {
            Position = newPosition;
        }
    }
}