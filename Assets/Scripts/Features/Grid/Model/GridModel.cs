using System.Collections.Generic;
using Game.Core.Data;
using Game.Services;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.Features.Grid.Model
{
    public class GridModel
    {
        private CellData[,] _cells;
        private GridConfig _gridConfig;
        private readonly RefillService _refillService;
        private int _spawnRows = 1;
        public int VisibleHeight { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public CellData[,] Cells =>_cells;


        public GridModel(GridConfig gridConfig)
        {
            _gridConfig = gridConfig;
            Width = gridConfig.Width;
            VisibleHeight = gridConfig.Height;
            Height = VisibleHeight + _spawnRows;
        }

        public void InitializeGrid()
        {
            _cells = new CellData[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height-1; y++)
                {
                    CellType randomType = GetRandomCellType();
                    Vector2Int position = new Vector2Int(x, y);
                    _cells[x, y] = new CellData(randomType, position);
                }
                int spawnY = Height - 1;
                _cells[x, spawnY] = new CellData(CellType.Empty, new Vector2Int(x, spawnY));
            }
        }
        
        public void SpawnCellAtTop(int x)
        {
            int spawnY = Height - 1;
        
            if (!_cells[x, spawnY].IsEmpty)
            {
                return;
            }

            CellType newType = GetRandomCellType();
            _cells[x, spawnY] = new CellData(newType, new Vector2Int(x, spawnY));
        }


        public CellData GetCell(int x, int y)
        {
            if (IsValidPosition(x, y)) 
                return _cells[x, y];
            return new CellData(CellType.Red, new Vector2Int(0, 0));

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

        public void SetCellType(int x, int y, CellType newType)
        {
            if (IsValidPosition(x, y))
            {
                _cells[x, y] = _cells[x, y].WithType(newType);
            }
        }
        
        public void ClearCell(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                SetCellType(x,y, CellType.Empty);
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
                Debug.Log(_cells[x,y].State == CellState.Idle);
                return _cells[x, y].CanClick;
            }
            return false;
        }

        private CellType GetRandomCellType()
        {
            int random = UnityEngine.Random.Range(0, _gridConfig.ColorCount);
            return random switch
            {
                0=> CellType.Red,
                1=>CellType.Blue,
                2=>CellType.Green,
                3=>CellType.Yellow,
                _ => CellType.Empty
            };

        }
        public void PrintGrid()
        {
            string output = "---Grid State---\n";
            for (int y = Height-1; y >=0; y--)
            {
                string row = $"Row {y}:";
                for (int x = 0; x < Width; x++)
                {
                    CellData cell = _cells[x, y];
                    string cellStr = cell.Type switch
                    {
                        CellType.Red => "R",
                        CellType.Blue => "B",
                        CellType.Green => "G",
                        CellType.Yellow => "Y",
                        CellType.Empty => "-",
                        _ => "?"
                    };
                    row += cellStr;
                }

                output += row + "\n";
            }
            Debug.Log(output);
        }
    }
}