using System;
using DG.Tweening;
using Game.Core.Data;
using Game.Services;
using UnityEngine;

namespace Game.Features.Grid.View
{
    public class GridView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _backgroundSprite;
        [SerializeField] private SpriteRenderer _maskSprite;
        [SerializeField] private Transform _gridContainer;

        [Header("Settings")]
        [SerializeField] private float _cellSize = 1f;

        public event Action<int, int> OnCellClicked;

        private PoolableObject[,] _cellObjects;
        private CellPoolService _cellPoolService;
        private int _width;
        private int _height;
        private int _totalHeight;

        public void Initialize(int width, int height, float cellSize, CellPoolService cellPoolService)
        {
            _width = width;
            _height = height;
            _totalHeight = height + 1;
            _cellSize = cellSize;
            _cellPoolService = cellPoolService;
            _cellObjects = new PoolableObject[_width, _totalHeight];
        }

        public void CreateGrid(CellData[,] slots)
        {
            ClearGrid();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    CreateCellVisual(x, y, slots[x, y]);
                }
            }
            UpdateBackgroundSize();
        }

        private void CreateCellVisual(int x, int y, CellData slot)
        {
            if (slot.IsEmpty) return;

            PoolableObject cellObj = _cellPoolService.Get(slot);
            if (cellObj == null) return;
            
            cellObj.name = $"Cell_{x}_{y}";
            cellObj.transform.SetParent(_gridContainer);
            cellObj.transform.position = CalculateWorldPosition(x, y);

            CellInput input = cellObj.GetComponent<CellInput>();
            if (input == null) input = cellObj.gameObject.AddComponent<CellInput>();
            
            input.Initialize(x, y, this);
            _cellObjects[x, y] = cellObj;
        }

        public void CreateCellAtSpawnPosition(int x, int spawnY, CellData slotData)
        {
            PoolableObject cellObj = _cellPoolService.Get(slotData);
            if (cellObj == null) return;

            cellObj.transform.SetParent(_gridContainer);
            cellObj.transform.position = CalculateWorldPosition(x, spawnY);
            cellObj.name = $"Cell_Spawn_{x}_{spawnY}";

            CellInput input = cellObj.GetComponent<CellInput>();
            if (input == null) input = cellObj.gameObject.AddComponent<CellInput>();

            input.Initialize(x, spawnY, this);
            _cellObjects[x, spawnY] = cellObj;
        }
        public void RemoveCell(int x, int y)
        {
            if (!IsValidPosition(x, y)) return;

            PoolableObject cellObj = _cellObjects[x, y];
            if (cellObj != null)
            {
                cellObj.ReturnToPool();
                _cellObjects[x, y] = null;
            }
        }
        public void MoveCellAnimated(int fromX, int fromY, int toX, int toY,
            float duration, float initialVelocity, float gravity, Action onComplete)
        {
            PoolableObject cellObj = _cellObjects[fromX, fromY];
            if (cellObj == null)
            {
                onComplete?.Invoke();
                return;
            }
            _cellObjects[toX, toY] = cellObj;
            _cellObjects[fromX, fromY] = null;
            Vector3 startPos = cellObj.transform.position;
            Vector3 targetPos = CalculateWorldPosition(toX, toY);
            startPos.z = targetPos.z;
            cellObj.transform.position = startPos;
    
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
                            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t01);
                            cellObj.transform.position = currentPos;
                        },
                        duration,
                        duration)
                    .SetEase(Ease.Linear)
            );

            sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
        private void UpdateBackgroundSize()
        {
            if (_backgroundSprite == null) return;

            float gridWidth = _width * _cellSize;
            float gridHeight = _height * _cellSize;
            _backgroundSprite.size = new Vector2(gridWidth + 0.2f, gridHeight + 0.2f);
            _maskSprite.size = new Vector2(gridWidth, gridHeight);
        }

        private Vector3 CalculateWorldPosition(int x, int y)
        {
            Vector3 offset = new Vector3(
                -(_width * _cellSize) / 2f + _cellSize / 2f,
                -(_height * _cellSize) / 2f + _cellSize / 2f,
                0
            );

            Vector3 position = new Vector3(
                offset.x + (x * _cellSize),
                offset.y + (y * _cellSize),
                -y
            );
            return position;
        }

        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _totalHeight;
        }

        public void HandleCellClick(int x, int y)
        {
            OnCellClicked?.Invoke(x, y);
        }
        private void ClearGrid()
        {
            if (_cellObjects == null) return;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _totalHeight; y++)
                {
                    if (_cellObjects[x, y] != null)
                    {
                        _cellObjects[x, y].ReturnToPool();
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