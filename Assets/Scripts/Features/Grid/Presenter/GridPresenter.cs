using System.Collections.Generic;
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
        private readonly GridConfig _config;

        public GridPresenter(GridModel gridModel, GridView gridView, MatchService matchService,
            PhysicsService physicsService, GridConfig config)
        {
            _gridModel = gridModel;
            _gridView = gridView;
            _matchService = matchService;
            _physicsService = physicsService;
            _config = config;
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
            if (matches.Count < _config.MinMatchCount)
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

            ApplyGravity();
        }
        private void ApplyGravity()
        {
            for (int x = 0; x < _gridModel.Width; x++)
            {
                for (int y = 0; y < _gridModel.Height; y++)
                {
                    CellData cell = _gridModel.GetCell(x, y);
                    if (cell.IsEmpty || cell.State == CellState.Moving)
                        continue;

                    if (y > 0 && _gridModel.GetCell(x, y - 1).IsEmpty)
                    {
                        StartFalling(x, y);
                    }
                }
            }
        }

        private void StartFalling(int x, int y)
        {
            _gridModel.SetCellState(x, y, CellState.Moving);
            FallOneStep(x, y);
        }

        private void FallOneStep(int x, int y)
        {
            CellData cell = _gridModel.GetCell(x, y);

            if (cell.IsEmpty)
            {
                return;
            }

            int targetY = y - 1;

            if (targetY < 0)
            {
                _gridModel.SetCellState(x, y, CellState.Idle);
                return;
            }
            float currentVelocity = _gridModel.GetCellVelocity(x, y);
            float fallDuration = _physicsService.CalculateFallDuration(currentVelocity);
            _gridModel.SetCell(x, targetY, cell.WithState(CellState.Moving));
            _gridModel.ClearCell(x, y);
            _gridModel.ClearCellVelocity(x, y);
            float newVelocity = _physicsService.CalculateVelocity(currentVelocity, fallDuration);
            _gridModel.SetCellVelocity(x, targetY, newVelocity);
            _gridView.MoveCellAnimated(x, y, x, targetY, fallDuration,currentVelocity,20f,
                onComplete: () => OnCellFallComplete(x, targetY)
            );
        }

        private void OnCellFallComplete(int x, int y)
        {
            if (y > 0 && _gridModel.GetCell(x, y - 1).IsEmpty)
            {
                FallOneStep(x, y);
            }
            else
            {
                _gridModel.SetCellState(x, y, CellState.Idle);
                _gridModel.ClearCellVelocity(x, y);
            }
        }
    }
}