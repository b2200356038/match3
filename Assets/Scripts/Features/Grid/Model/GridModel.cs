using System;
using System.Collections.Generic;
using Game.Core.Data;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.Features.Grid.Model
{
    public class GridModel
    {
        public event Action<CellData[,]> OnGridChanged;
        public event Action<List<Vector2Int>> OnCellsMathced;
        public event Action<List<Vector2Int>> OnCellsRemoved;

        private CellData[,] _cells;
        private GridConfig _config;
        
        public int Width { get; private set; }
        public int Height { get; private set; }
        public CellData[,] Cells =>_cells;
        
        private Dictionary<Vector2Int, float> _cellVelocities = new Dictionary<Vector2Int, float>();


        public GridModel(GridConfig config)
        {
            _config = config;
            Width = _config.Width;
            Height = _config.Height;
        }

        public void InitializeGrid()
        {
            _cells = new CellData[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    CellType randomType = GetRandomCellType();
                    Vector2Int position = new Vector2Int(x, y);
                    _cells[x, y] = new CellData(randomType, position);
                }
            }
            OnGridChanged?.Invoke(_cells);
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
                OnGridChanged?.Invoke(_cells);
            }
        }
        
        public void SetCellState(int x, int y, CellState newState)
        {
            if (IsValidPosition(x, y))
            {
                _cells[x, y] = _cells[x, y].WithState(newState);
                OnGridChanged?.Invoke(_cells);
            }
        }

        public void SetCellType(int x, int y, CellType newType)
        {
            if (IsValidPosition(x, y))
            {
                _cells[x, y] = _cells[x, y].WithType(newType);
                OnGridChanged?.Invoke(_cells);
            }
        }
        
        public void ClearCell(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                SetCellType(x,y, CellType.Empty);
                OnGridChanged?.Invoke(_cells);
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

        private CellType GetRandomCellType()
        {
            int random = UnityEngine.Random.Range(0, _config.ColorCount);
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
        public float GetCellVelocity(int x, int y)
        {
            Vector2Int key = new Vector2Int(x, y);
            return _cellVelocities.ContainsKey(key) ? _cellVelocities[key] : 0f;
        }
        public void SetCellVelocity(int x, int y, float velocity)
        {
            Vector2Int key = new Vector2Int(x, y);
            _cellVelocities[key] = velocity;
        }
        public void ClearCellVelocity(int x, int y)
        {
            Vector2Int key = new Vector2Int(x, y);
            _cellVelocities.Remove(key);
        }
    }
}