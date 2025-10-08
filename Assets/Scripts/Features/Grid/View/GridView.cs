using System;
using DG.Tweening;
using Game.Core.Data;
using UnityEngine;

namespace Game.Features.Grid.View
{
    public class GridView : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private Transform _gridContainer;

        [Header("Settings")] 
        [SerializeField] private float _cellSize = 1f;

        public event Action<int, int> OnCellClicked;

        private GameObject[,] _cellObjects;
        private int _width;
        private int _height;           // Görünen height
        private int _totalHeight;      // Görünen + spawn row

        public void Initialize(int width, int height, float cellSize)
        {
            _width = width;
            _height = height;           // Görünen kısım (8)
            _totalHeight = height + 1;  // Spawn row dahil (9)
            _cellSize = cellSize;
            _cellObjects = new GameObject[_width, _totalHeight];
            Debug.Log($"GridView initialized: {width}x{height} (total: {_totalHeight})");
        }

        public void CreateGrid(CellData[,] cells)
        {
            ClearGrid();
            for (int x = 0; x < _width; x++)
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
            Debug.Log("Grid created");
        }

        public void CreateCellAtSpawnPosition(int x, int spawnY, CellData cellData)
        {
            Vector3 spawnPos = CalculateWorldPosition(x, spawnY);
            GameObject cellObj = Instantiate(_cellPrefab, _gridContainer);
            cellObj.transform.position = spawnPos;
            cellObj.name = $"Cell_Spawn_{x}_{spawnY}";
            SetCellColor(cellObj, cellData.Type);
            CellInput input = cellObj.GetComponent<CellInput>();
            if (input == null)
            {
                input = cellObj.AddComponent<CellInput>();
            }
            input.Initialize(x, spawnY, this);
            _cellObjects[x, spawnY] = cellObj;
        }

        public void RemoveCell(int x, int y)
        {
            if (!IsValidPosition(x, y))
            {
                return;
            }

            GameObject cellObj = _cellObjects[x, y];
            if (cellObj != null)
            {
                Destroy(cellObj);
                _cellObjects[x, y] = null;
            }
        }

        public void MoveCellAnimated(int fromX, int fromY, int toX, int toY, 
            float duration, float initialVelocity, float gravity, Action onComplete)
        {
            GameObject cellObj = _cellObjects[fromX, fromY];
            if (cellObj == null)
            {
                onComplete?.Invoke();
                return;
            }
            
            _cellObjects[toX, toY] = cellObj;
            _cellObjects[fromX, fromY] = null;
            Vector3 startPos = cellObj.transform.position;
            Vector3 targetPos = CalculateWorldPosition(toX, toY);
            float distance = Vector3.Distance(startPos, targetPos);
            cellObj.name = $"Cell_{toX}_{toY}";
            CellInput input = cellObj.GetComponent<CellInput>();
            if (input != null)
            {
                input.Initialize(toX, toY, this);
            }
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                DOTween.To(() => 0f, t =>
                    {
                        float currentDistance = initialVelocity * t + 0.5f * gravity * t * t;
                        float t01 = currentDistance / distance;
                        t01 = Mathf.Clamp01(t01);
                        cellObj.transform.position = Vector3.Lerp(startPos, targetPos, t01);
                    },
                    duration,
                    duration)
                    .SetEase(Ease.Linear)
            );
            
            sequence.OnComplete(() =>
            {
                cellObj.transform.position = targetPos; 
                onComplete?.Invoke();
            });
        }
        
        public void UpdateCell(int x, int y, CellData cellData)
        {
            if (!IsValidPosition(x, y))
            {
                return;
            }

            GameObject cellObj = _cellObjects[x, y];

            if (cellData.IsEmpty)
            {
                RemoveCell(x, y);
            }
            else
            {
                if (cellObj == null)
                {
                    CreateCellVisual(x, y, cellData);
                }
                else
                {
                    SetCellColor(cellObj, cellData.Type);
                }
            }
        }
        
        private void CreateCellVisual(int x, int y, CellData cell)
        {
            GameObject cellObj = Instantiate(_cellPrefab, _gridContainer);
            cellObj.name = $"Cell_{x}_{y}";
            
            Vector3 worldPos = CalculateWorldPosition(x, y);
            cellObj.transform.position = worldPos;
            
            SetCellColor(cellObj, cell.Type);
            
            CellInput input = cellObj.GetComponent<CellInput>();
            if (input == null)
            {
                input = cellObj.AddComponent<CellInput>();
            }
            input.Initialize(x, y, this);
            
            _cellObjects[x, y] = cellObj;
        }
        
        private void SetCellColor(GameObject cellObj, CellType cellType)
        {
            SpriteRenderer spriteRenderer = cellObj.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                return;
            }
            
            Color color = cellType switch
            {
                CellType.Red => Color.red,
                CellType.Green => Color.green,
                CellType.Blue => Color.blue,
                CellType.Yellow => Color.yellow,
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
            
            return new Vector3(
                offset.x + (x * _cellSize), 
                offset.y + (y * _cellSize),  
                0
            );
        }
        
        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _totalHeight;
        }
        
        public void HandleCellClick(int x, int y)
        {
            Debug.Log($"Cell clicked: ({x}, {y})");
            OnCellClicked?.Invoke(x, y);
        }
        
        private void ClearGrid()
        {
            if (_cellObjects == null)
            {
                return;
            }
            
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _totalHeight; y++)
                {
                    if (_cellObjects[x, y] != null)
                    {
                        Destroy(_cellObjects[x, y]);
                        _cellObjects[x, y] = null;
                    }
                }
            }
        }
        
        private void OnDestroy()
        {
            ClearGrid();
        }
    }
}