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
        private readonly CascadeService _cascadeService;
        private readonly ObstacleService _obstacleService;
        private readonly PowerUpService _powerUpService;
        private readonly PhysicsService _physicsService;
        private readonly GridConfig _gridConfig;
        public GridPresenter(GridModel gridModel, GridView gridView, MatchService matchService,
            CascadeService cascadeService, ObstacleService obstacleService, PowerUpService powerUpService,
            PhysicsService physicsService, GridConfig gridConfig)
        {
            _gridModel = gridModel;
            _gridView = gridView;
            _matchService = matchService;
            _cascadeService = cascadeService;
            _obstacleService = obstacleService;
            _powerUpService = powerUpService;
            _physicsService = physicsService;
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
                return;
            CellData clickedCell = _gridModel.GetCell(x, y);
            if (clickedCell.IsPowerUp)
                return; // TODO: Handle PowerUp activation
            if (!clickedCell.IsCube)
                return;
            var matches = _matchService.FindMatches(x, y);
            if (matches.Count < _gridConfig.MinMatchCount)
                return;
            ProcessMatches(matches);
        }
        private void ProcessMatches(List<Vector2Int> matches)
        {
            List<Vector2Int> damagedObstacles = _obstacleService.GetAffectedObstacles(matches);
            DamageObstacles(damagedObstacles);
            if (_powerUpService.TryGetPowerUp(matches, out PowerUpType powerUpType))
            {
                CreatePowerUp(matches, powerUpType);
            }
            else
            {
                foreach (var match in matches)
                {
                    _gridModel.ClearCell(match.x, match.y);
                    _gridView.RemoveCell(match.x, match.y);
                }
            }
        }
        private void DamageObstacles(List<Vector2Int> obstacles)
        {
            foreach (var obstaclePos in obstacles)
            {
                CellData damagedObstacle = _gridModel.GetCell(obstaclePos.x, obstaclePos.y).TakeDamage();

                if (damagedObstacle.Health <= 0)
                {
                    _gridModel.ClearCell(obstaclePos.x, obstaclePos.y);
                    _gridView.RemoveCell(obstaclePos.x, obstaclePos.y);
                }
                else
                {
                    _gridModel.SetCell(obstaclePos.x, obstaclePos.y, damagedObstacle);
                    //TODO: gridview damage animation
                }
            }
        }
        private void CreatePowerUp(List<Vector2Int> matches, PowerUpType powerUpType)
        {
            foreach (var match in matches)
            {
                _gridModel.SetCellState(match.x, match.y, CellState.Matched);
            }
            _gridView.CreatePowerUp(matches, () =>
            {
                foreach (var match in matches)
                {
                    _gridModel.ClearCell(match.x, match.y);
                }
                CellData powerUp = _gridModel.SetPowerUp(matches[0], powerUpType);
                _gridView.CreateCell(matches[0].x, matches[0].y, powerUp);
            });
        }
    }
}