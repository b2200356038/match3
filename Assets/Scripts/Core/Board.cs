using System.Collections.Generic;
using Game.Core.Data;
using Game.Services;
using UnityEngine;
using DG.Tweening;

namespace Game.Core
{
    public class Board : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _gridContainer;
        [SerializeField] private SpriteRenderer _backgroundSprite;
        [SerializeField] private SpriteRenderer _maskSprite;

        private Cell[,] _cells;
        private GridConfig _gridConfig;
        private CellFactory _cellFactory;
        private MatchService _matchService;
        private PhysicsService _physicsService;

        public int Width { get; private set; }
        public int VisibleHeight { get; private set; }
        public int TotalHeight { get; private set; }
        public float CellSize { get; private set; }

        private List<Vector2Int> _spawnerPositions = new();
        private bool _inputEnabled = true;

        public void Initialize(GridConfig gridConfig, CellConfig cellConfig)
        {
            _gridConfig = gridConfig;
            Width = gridConfig.Width;
            VisibleHeight = gridConfig.VisibleHeight;
            TotalHeight = gridConfig.TotalHeight;
            CellSize = gridConfig.CellSize;

            _cellFactory = new CellFactory(cellConfig);
            _matchService = new MatchService(gridConfig.MinMatchCount);
            _physicsService = new PhysicsService(gridConfig.gravity, gridConfig.CellSize);

            if (_gridContainer == null)
            {
                var containerObj = new GameObject("GridContainer");
                _gridContainer = containerObj.transform;
                _gridContainer.SetParent(transform);
                _gridContainer.localPosition = Vector3.zero;
            }

            InitializeGrid();
            UpdateBackgroundSize();
        }

        private void InitializeGrid()
        {
            _cells = new Cell[Width, TotalHeight];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < VisibleHeight; y++)
                {
                    var cell = _cellFactory.CreateRandomCube(new Vector2Int(x, y));
                    if (cell != null)
                    {
                        _cells[x, y] = cell;
                        cell.transform.position = GetWorldPosition(x, y);
                        cell.transform.SetParent(_gridContainer);
                        
                        AddCellInput(cell, x, y);
                    }
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int spawnRow = 0; spawnRow < _gridConfig.SpawnRows; spawnRow++)
                {
                    int spawnY = VisibleHeight + spawnRow;
                    _spawnerPositions.Add(new Vector2Int(x, spawnY));
                }
            }
        }

        private void UpdateBackgroundSize()
        {
            if (_backgroundSprite == null) return;

            float gridWidth = Width * CellSize;
            float gridHeight = VisibleHeight * CellSize;
            
            _backgroundSprite.size = new Vector2(gridWidth + 0.2f, gridHeight + 0.2f);
            
            if (_maskSprite != null)
            {
                _maskSprite.size = new Vector2(gridWidth, gridHeight);
            }
        }

        private void AddCellInput(Cell cell, int x, int y)
        {
            var cellInput = cell.GetComponent<CellInput>();
            if (cellInput == null)
            {
                cellInput = cell.gameObject.AddComponent<CellInput>();
            }
            cellInput.Initialize(x, y, this);
        }

        public void HandleCellClick(int x, int y)
        {
            if (!_inputEnabled) return;
            
            if (y >= VisibleHeight) return;
            
            Cell cell = _cells[x, y];
            
            if (cell == null) return;
            if (cell.State != CellState.Idle) return;

            if (cell is CubeCell)
            {
                ProcessMatch(x, y);
            }
            else if (cell is PowerUpCell powerUp)
            {
            }
        }

        private void ProcessMatch(int x, int y)
        {
            var matches = _matchService.FindMatches(_cells, Width, TotalHeight, x, y);

            if (matches.Count < _gridConfig.MinMatchCount)
                return;
            DamageNearbyObstacles(matches);
            if (TryCreatePowerUp(matches, out PowerUpType powerUpType))
            {
                CreatePowerUp(matches, powerUpType);
            }
            else
            {
                DeleteMatches(matches);
            }
        }

        private bool TryCreatePowerUp(List<Vector2Int> matches, out PowerUpType powerUpType)
        {
            powerUpType = PowerUpType.Bomb;
            if (matches.Count < 4)
                return false;
            if (matches.Count >= 7)
            {
                powerUpType = PowerUpType.Bomb;
                return true;
            }
            int random = Random.Range(0, 2);
            powerUpType = random == 0 ? PowerUpType.RowRocket : PowerUpType.ColumnRocket;
            return true;
        }

        private void CreatePowerUp(List<Vector2Int> matches, PowerUpType powerUpType)
        {
            Vector2Int originPos = matches[0];
            foreach (var pos in matches)
            {
                Cell cell = _cells[pos.x, pos.y];
                if (cell != null)
                {
                    cell.State = CellState.Matched;
                }
            }
            AnimatePowerUpCreation(matches, originPos, () =>
            {
                foreach (var match in matches)
                {
                    if (match == originPos) continue;
                    Cell cell = _cells[match.x, match.y];
                    if (cell != null)
                    {
                        var cellInput = cell.GetComponent<CellInput>();
                        if (cellInput != null)
                        {
                            cellInput.Disable();
                        }

                        Destroy(cell.gameObject);
                        _cells[match.x, match.y] = null;
                    }
                    CheckAbove(match.x, match.y + 1, velocity: 0f);
                }
                CreatePowerUpAt(originPos.x, originPos.y, powerUpType);
            });
        }

        private void AnimatePowerUpCreation(List<Vector2Int> matches, Vector2Int targetPos, System.Action onComplete)
        {
            Cell targetCell = _cells[targetPos.x, targetPos.y];
            if (targetCell == null)
            {
                onComplete?.Invoke();
                return;
            }

            Vector3 targetWorldPos = targetCell.transform.position;
            int animationCount = matches.Count;
            int completedCount = 0;

            foreach (var pos in matches)
            {
                Cell cell = _cells[pos.x, pos.y];
                if (cell == null) continue;

                var cellInput = cell.GetComponent<CellInput>();
                if (cellInput != null)
                {
                    cellInput.Disable();
                }
                cell.transform.DOMove(targetWorldPos, 0.2f)
                    .SetEase(Ease.InSine)
                    .OnComplete(() =>
                    {
                        if (cell != targetCell)
                        {
                            cell.gameObject.SetActive(false);
                        }
                        completedCount++;
                        if (completedCount >= animationCount)
                        {
                            onComplete?.Invoke();
                        }
                    });
            }
        }

        private void CreatePowerUpAt(int x, int y, PowerUpType powerUpType)
        {
            Cell oldCell = _cells[x, y];
            if (oldCell != null)
            {
                Destroy(oldCell.gameObject);
            }
            var powerUpCell = _cellFactory.CreatePowerUp(powerUpType, new Vector2Int(x, y));
            if (powerUpCell != null)
            {
                _cells[x, y] = powerUpCell;
                powerUpCell.transform.position = GetWorldPosition(x, y);
                powerUpCell.transform.SetParent(_gridContainer);

                AddCellInput(powerUpCell, x, y);
            }
        }

        private void DeleteMatches(List<Vector2Int> matches)
        {
            foreach (var pos in matches)
            {
                Cell cell = _cells[pos.x, pos.y];
                if (cell != null)
                {
                    cell.State = CellState.Matched;

                    var cellInput = cell.GetComponent<CellInput>();
                    if (cellInput != null)
                    {
                        cellInput.Disable();
                    }

                    Destroy(cell.gameObject);
                    _cells[pos.x, pos.y] = null;
                }
            }

            foreach (var match in matches)
            {
                CheckAbove(match.x, match.y + 1, velocity: 0f);
            }
        }

        private void DamageNearbyObstacles(List<Vector2Int> matches)
        {
            HashSet<Vector2Int> checkedPositions = new HashSet<Vector2Int>();

            Vector2Int[] directions = {
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0)
            };

            foreach (var match in matches)
            {
                foreach (var dir in directions)
                {
                    Vector2Int pos = match + dir;

                    if (checkedPositions.Contains(pos)) continue;
                    if (!IsValidPosition(pos.x, pos.y)) continue;

                    Cell cell = _cells[pos.x, pos.y];
                    if (cell is ObstacleCell obstacle)
                    {
                        obstacle.TakeDamage();
                        checkedPositions.Add(pos);

                        if (obstacle.Health <= 0)
                        {
                            _cells[pos.x, pos.y] = null;
                            CheckAbove(pos.x, pos.y + 1, velocity: 0f);
                        }
                    }
                }
            }
        }

        private void CheckAbove(int x, int y, float velocity)
        {
            if (!IsValidPosition(x, y))
                return;

            Cell currentCell = _cells[x, y];

            if (currentCell != null && currentCell.CanFall && currentCell.State == CellState.Idle)
            {
                currentCell.State = CellState.Falling;
                FallOneStep(x, y, velocity);
            }
            else if (currentCell == null && _spawnerPositions.Contains(new Vector2Int(x, y)))
            {
                SpawnNewCell(x, y);
                FallOneStep(x, y, velocity);
            }
        }

        private bool CanCellFall(int x, int y)
        {
            if (!IsValidPosition(x, y))
                return false;

            Cell cell = _cells[x, y];
            if (cell == null || !cell.CanFall)
                return false;

            if (y > 0 && _cells[x, y - 1] == null)
                return true;

            return false;
        }

        private void SpawnNewCell(int x, int y)
        {
            var newCell = _cellFactory.CreateRandomCube(new Vector2Int(x, y));
            if (newCell != null)
            {
                _cells[x, y] = newCell;
                newCell.transform.SetParent(_gridContainer);
                newCell.transform.position = GetWorldPosition(x, y);

                AddCellInput(newCell, x, y);
            }
        }

        private void FallOneStep(int x, int y, float velocity)
        {
            int targetY = y - 1;
            Cell cell = _cells[x, y];

            _cells[x, targetY] = cell;
            _cells[x, targetY].State = CellState.Falling;
            _cells[x, y] = null;

            cell.MoveTo(new Vector2Int(x, targetY));

            var cellInput = cell.GetComponent<CellInput>();
            if (cellInput != null)
            {
                cellInput.Initialize(x, targetY, this);
            }

            float duration = _physicsService.CalculateFallDuration(velocity);
            float nextVelocity = _physicsService.CalculateVelocity(velocity, duration);

            DOVirtual.DelayedCall(_gridConfig.cascadeDelay, () => CheckAbove(x, y + 1, velocity));

            MoveCellAnimated(x, y, x, targetY, duration, velocity,
                onComplete: () => OnCellLanded(x, targetY, nextVelocity));
        }

        private void MoveCellAnimated(int fromX, int fromY, int toX, int toY,
            float duration, float initialVelocity, System.Action onComplete)
        {
            Cell cell = _cells[toX, toY];
            if (cell == null)
            {
                onComplete?.Invoke();
                return;
            }

            Vector3 startPos = GetWorldPosition(fromX, fromY);
            Vector3 targetPos = GetWorldPosition(toX, toY);
            startPos.z = targetPos.z;

            cell.transform.position = startPos;

            float distance = Vector3.Distance(startPos, targetPos);

            DOTween.To(() => 0f, t =>
            {
                float currentDistance = initialVelocity * t + 0.5f * _gridConfig.gravity * t * t;
                float t01 = currentDistance / distance;
                t01 = Mathf.Clamp01(t01);
                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t01);
                cell.transform.position = currentPos;
            },
            duration,
            duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => onComplete?.Invoke());
        }

        private void OnCellLanded(int x, int y, float nextVelocity)
        {
            Cell cell = _cells[x, y];
            if (cell == null) return;

            if (CanCellFall(x, y))
            {
                FallOneStep(x, y, nextVelocity);
            }
            else
            {
                cell.State = CellState.Idle;
            }
        }

        public Cell GetCell(int x, int y)
        {
            if (IsValidPosition(x, y))
                return _cells[x, y];
            return null;
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < TotalHeight;
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            float offsetX = -(Width * CellSize) / 2f + CellSize / 2f;
            float offsetY = -(VisibleHeight * CellSize) / 2f + CellSize / 2f;

            return new Vector3(
                offsetX + (x * CellSize),
                offsetY + (y * CellSize),
                -y
            );
        }

        public void ToggleInput(bool enabled)
        {
            _inputEnabled = enabled;
        }
    }
}