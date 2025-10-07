using System;
using Game.Core.Data;
using UnityEngine;

namespace Game.Features.Grid.View
{
    public class GridView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject _cellPrefab;
        [SerializeField]
        private Transform _gridContainer;

        [Header("Settings")] 
        [SerializeField] 
        private float _cellSize = 1f;

        public event System.Action<int, int> OnCellClicked;

        private GameObject[,] _cellObjects;
        private int _width;
        private int _height;

        public void Initialize(int width, int height, float cellSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _cellObjects = new GameObject[_width,_height];
            Debug.Log(($"Grid initialized: {width}, {height}"));
        }

        public void RenderGrid(CellData[,] cells)
        {
            ClearGrid();
            for (int x = 0; x < _height; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    CellData cell = cells[x, y];
                    if (cell.IsEmpty)
                    {
                        continue;
                    }
                    CreateCellVisual(x, y, cell);
                }
            }
            Debug.Log("Grid rendered");
        }

        private void CreateCellVisual(int x, int y, CellData cell)
        {
            GameObject cellObj = Instantiate(_cellPrefab, _gridContainer);
            cellObj.name = $"Cell_{x}_{y}";
            Vector3 worldPos = CalculateWorldPosition(x, y);
            cellObj.transform.position = worldPos;
            SetCellColor(cellObj, cell.Type);
            CellInput input = cellObj.GetComponent<CellInput>();
            if (input==null)
            {
                input = cellObj.AddComponent<CellInput>();
            }
            input.Initialize(x, y, this);
            _cellObjects[x, y] = cellObj;
        }

        private void SetCellColor(GameObject cellObj, CellType cellType)
        {
            SpriteRenderer spriteRenderer = cellObj.GetComponent<SpriteRenderer>();
            if (spriteRenderer==null)
            {
                Debug.LogError("Cell prefab needs SpriteRenderer");
                return;
            }

            Color color = cellType switch
            {
                CellType.Red => Color.red,
                CellType.Green => Color.green,
                CellType.Blue=>Color.blue,
                CellType.Yellow=>Color.yellow,
                _ => Color.white
            };
            spriteRenderer.color = color;
        }

        private Vector3 CalculateWorldPosition(int x, int y)
        {
            Vector3 offset = new Vector3(
                -(_width * _cellSize) / 2f + _cellSize / 2f,
                -(_height * _cellSize) / 2f + _cellSize / 2f,
                0
            );
            return new Vector3(offset.x + (x * _cellSize), offset.y + (y * _cellSize));
        }

        public void HandleCellClick(int x, int y)
        {
            Debug.Log($"Cell clicked: {x}, {y}");
            OnCellClicked?.Invoke(x,y);
        }

        private void ClearGrid()
        {
            if (_cellObjects==null)
            {
                return;
            }
            for(int x=0; x<_width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Destroy(_cellObjects[x,y]);
                    _cellObjects[x, y] = null;
                }
            }
        }

        private void OnDestroy()
        {
            ClearGrid();
        }
    }
}