using UnityEngine;

namespace Game.Core.Data
{
    public struct CellData
    {
        public CellType Type { get; private set; }
        public Vector2Int Position { get; private set; }
        public CellState State { get; private set; }

        public CellData(CellType type, Vector2Int position, CellState state = CellState.Idle)
        {
            Type = type;
            Position = position;
            State = state;
        }

        public bool IsEmpty => Type == CellType.Empty;
        public bool IsPlayable => Type != CellType.Empty;
        public bool CanClick => State == CellState.Idle && IsPlayable;

        public CellData WithState(CellState newState)
        {
            return new CellData(Type, Position, newState);
        }

        public CellData WithType(CellType newType)
        {
            return new CellData(newType, Position, State);
        }

        public override string ToString()
        {
            return $"Cell[{Position.x},{Position.y}: {Type} ({State})]";
        }
    }
}