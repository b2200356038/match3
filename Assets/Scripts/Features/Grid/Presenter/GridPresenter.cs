using System.Collections.Generic;
using DG.Tweening;
using Game.Core.Data;
using Game.Features.Grid.Model;
using Game.Features.Grid.View;
using Game.Services;
using UnityEngine;

namespace Game.Features.Grid.Presenter
{
    public class GridPresenter
    {
        private readonly GridModel _gridModel;
        private readonly GridView _gridView;
        private readonly MatchService _matchService;
        private readonly PhysicsService _physicsService;
        private readonly RefillService _refillService;
        private readonly GridConfig _gridConfig;

        public GridPresenter(GridModel gridModel, GridView gridView, MatchService matchService,
            PhysicsService physicsService, RefillService refillService, GridConfig gridConfig)
        {
            _gridModel = gridModel;
            _gridView = gridView;
            _matchService = matchService;
            _physicsService = physicsService;
            _refillService = refillService;
            _gridConfig = gridConfig;
        }

        public void Initialize()
        {
            _gridView.OnCellClicked += HandleCellClick;
        }

        public void Dispose()
        {
            _gridView.OnCellClicked -= HandleCellClick;
        }

        public void StartGame()
        {
            _gridModel.InitializeGrid();
            _gridView.CreateGrid(_gridModel.Cells);
        }

        private void HandleCellClick(int x, int y)
        {
            if (!_gridModel.IsCellClickable(x, y))
            {
                return;
            }

            var matches = _matchService.FindMatches(x, y);
            if (matches.Count < _gridConfig.MinMatchCount)
            {
                return;
            }

            ProcessMatches(matches);
        }

        private void ProcessMatches(List<Vector2Int> matches)
        {
            foreach (var pos in matches)
            {
                _gridModel.ClearCell(pos.x, pos.y);
                _gridView.RemoveCell(pos.x, pos.y);
            }
            foreach (var match in matches)
            {
                CheckAbove(match.x, match.y, 0);
            }
        }

        private void CheckAbove(int x, int y , float initialVelocity)
        {
            int aboveY = y + 1;
    
            if (aboveY >= _gridModel.Height)
            {
                return;
            }
            Debug.Log($"({x},{y}");
            DOVirtual.DelayedCall(_gridConfig.cascadeDelay, () =>
            {
                if (aboveY == _gridModel.VisibleHeight)
                {
                    CellData spawnCell = _gridModel.GetCell(x, aboveY);
                    if (spawnCell.IsEmpty)
                    {
                        SpawnNewCellAtTop(x);
                        FallOneStep(x, aboveY,initialVelocity);
                        
                    }
                }
                CellData aboveCell = _gridModel.GetCell(x, aboveY);
                if (!aboveCell.IsEmpty && aboveCell.State != CellState.Moving)
                {
                    StartFalling(x, aboveY);
                }
            });
        }

        private void StartFalling(int x, int y)
        {
            _gridModel.SetCellState(x, y, CellState.Moving);
            FallOneStep(x, y, 0);
        }

        private void FallOneStep(int x, int y, float initialVelocity)
        {
            CheckAbove(x, y, initialVelocity);
            CellData cell = _gridModel.GetCell(x, y);
            if (cell.IsEmpty)
            {
                Debug.Log("return");
                return;
            }
            int targetY = y - 1;
            if (targetY < 0)
            {
                _gridModel.SetCellState(x, y, CellState.Idle);
                return;
            }
            _gridModel.SetCell(x, targetY, cell.WithState(CellState.Moving));
            _gridModel.ClearCell(x, y);
            float fallDuration = _physicsService.CalculateFallDuration(initialVelocity);
            float newVelocity = initialVelocity + _gridConfig.gravity * fallDuration;
            
            _gridView.MoveCellAnimated(
                x, y,  
                x, targetY, 
                fallDuration, 
                initialVelocity, 
                _gridConfig.gravity,
                onComplete: () => OnCellFallComplete(x, targetY, newVelocity)
            );
        }

        private void OnCellFallComplete(int x, int y, float initialVelocity)
        {
            if (y > 0 && _gridModel.GetCell(x, y - 1).IsEmpty)
            {
                FallOneStep(x, y, initialVelocity); 
            }
            else
            {
                _gridModel.SetCellState(x, y, CellState.Idle);
            }
        }

        private void SpawnNewCellAtTop(int x)
        {
            _gridModel.SpawnCellAtTop(x);
            int spawnY = _gridModel.Height - 1;
            CellData spawnedCell = _gridModel.GetCell(x, spawnY);
            _gridView.CreateCellAtSpawnPosition(x, spawnY, spawnedCell);
        }
    }
}