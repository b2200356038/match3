using System.Collections.Generic;
using Game.Core.Data;
using UnityEngine;

namespace Game.Features.Grid.Model
{
    public class GridModel
    {
        private CellData[,] _slots;
        private GridConfig _gridConfig;
        private int _spawnRows = 1;

        public int VisibleHeight { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public CellData[,] Slots => _slots;

        public GridModel(GridConfig gridConfig)
        {
            _gridConfig = gridConfig;
            Width = gridConfig.Width;
            VisibleHeight = gridConfig.Height;
            Height = VisibleHeight + _spawnRows;
        }

        public void InitializeGrid()
        {
            _slots = new CellData[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height - 1; y++)
                {
                    CubeType randomCube = GetRandomCubeType();
                    _slots[x, y] = new CellData(randomCube, new Vector2Int(x, y));
                }

                int spawnY = Height - 1;
                _slots[x, spawnY] = CellData.CreateEmpty(new Vector2Int(x, spawnY));
            }
        }

        public void SpawnCellAtTop(int x)
        {
            int spawnY = Height - 1;

            if (!_slots[x, spawnY].IsEmpty)
            {
                return;
            }

            CubeType cubeType = GetRandomCubeType();
            _slots[x, spawnY] = new CellData(cubeType, new Vector2Int(x, spawnY));
        }

        public CellData GetSlot(int x, int y)
        {
            if (IsValidPosition(x, y))
                return _slots[x, y];
            return CellData.CreateEmpty(new Vector2Int(0, 0));
        }

        public void SetSlot(int x, int y, CellData slotData)
        {
            if (IsValidPosition(x, y))
            {
                _slots[x, y] = slotData;
            }
        }

        public void SetSlotState(int x, int y, CellState newState)
        {
            if (IsValidPosition(x, y))
            {
                _slots[x, y] = _slots[x, y].WithState(newState);
            }
        }

        public void ClearSlot(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _slots[x, y] = CellData.CreateEmpty(new Vector2Int(x, y));
            }
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool IsSlotClickable(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _slots[x, y].CanClick;
            }

            return false;
        }

        private CubeType GetRandomCubeType()
        {
            int random = Random.Range(0, _gridConfig.ColorCount);
            return random switch
            {
                0 => CubeType.Red,
                1 => CubeType.Blue,
                2 => CubeType.Green,
                3 => CubeType.Yellow,
                _ => CubeType.Red
            };
        }
    }
}