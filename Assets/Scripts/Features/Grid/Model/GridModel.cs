using System.Collections.Generic;
using Game.Core.Data;
using Game.Services;
using UnityEngine;

namespace Game.Features.Grid.Model
{
    public class GridModel
    {
        private CellData[,] _cells;
        private GridConfig _gridConfig;
        private CellFactory _cellFactory;
        private int _spawnRows = 1;

        public int VisibleHeight { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public CellData[,] Cells => _cells;

        public GridModel(GridConfig gridConfig, CellFactory cellFactory)
        {
            _gridConfig = gridConfig;
            _cellFactory = cellFactory;
            Width = gridConfig.Width;
            VisibleHeight = gridConfig.Height;
            Height = VisibleHeight + _spawnRows;
        }

        public void InitializeGrid()
        {
            _cells = new CellData[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height - 2; y++)
                {
                    _cells[x, y] = _cellFactory.CreateRandomCube(new Vector2Int(x,y));
                }
                int spawnY = Height - 2;
                _cells[x, spawnY] = _cellFactory.CreateObstacle(ObstacleType.Rock, new Vector2Int(x, spawnY));
                spawnY++;
                _cells[x, spawnY] = CellData.CreateEmpty(new Vector2Int(x, spawnY));
            }
        }

        public void RefillCellAtTop(int x)
        {
            int spawnY = Height - 1;

            if (!_cells[x, spawnY].IsEmpty)
            {
                return;
            }
            _cells[x, spawnY] = _cellFactory.CreateRandomCube(new Vector2Int(x, spawnY));
        }
        
        public CellData SpawnPowerUp(PowerUpType powerUpType, Vector2Int position )
        {
            _cells[position.x, position.y] = _cellFactory.CreatePowerUp(powerUpType, position);
            return _cells[position.x, position.y];
        }

        public CellData GetCell(int x, int y)
        {
            if (IsValidPosition(x, y))
                return _cells[x, y];
            return _cellFactory.CreateEmpty( new Vector2Int(x, y));
        }

        public void SetCell(int x, int y, CellData cellData)
        {
            if (IsValidPosition(x, y))
            {
                _cells[x, y] = cellData;
            }
        }

        public void SetCellState(int x, int y, CellState newState)
        {
            if (IsValidPosition(x, y))
            {
                _cells[x, y] = _cells[x, y].WithState(newState);
            }
        }

        public void SetCellFallable(CellData cellData)
        {
            _cells[cellData.Position.x, cellData.Position.y] = cellData;
        }

        public void ClearCell(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _cells[x, y] = _cellFactory.CreateEmpty( new Vector2Int(x, y));
            }
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool IsCellClickable(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _cells[x, y].CanClick;
            }

            return false;
        }
    }
}