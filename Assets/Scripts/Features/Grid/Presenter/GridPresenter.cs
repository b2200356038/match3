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
                return;
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
                foreach (var match in matches)
                {
                    CheckAbove(match.x, match.y + 1, velocity: 0f);
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
                    CheckAbove(obstaclePos.x, obstaclePos.y + 1, velocity: 0f);
                }
                else
                {
                    _gridModel.SetCell(obstaclePos.x, obstaclePos.y, damagedObstacle);
                    //TODO: gridview animation
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
                    if (match==matches[0])
                    {
                        CellData powerUp = _gridModel.SetPowerUp(matches[0], powerUpType);
                        _gridView.CreateCell(matches[0].x, matches[0].y, powerUp);
                        continue;
                    }
                    _gridModel.ClearCell(match.x, match.y);
                    CheckAbove(match.x, match.y + 1, velocity: 0f);
                } 
            });
        }
        private void CheckAbove(int x, int y, float velocity)
        {
            if (y == _gridModel.Height - 1)
            {
                CellData spawnCell = _gridModel.GetCell(x, y);
                if (spawnCell.IsEmpty)
                {
                    SpawnNewCell(x, y);
                    FallOneStep(x,y,velocity);
                    return;
                }
            }
            if (_cascadeService.CanCellFall(x, y) && _gridModel.GetCell(x,y).State == CellState.Idle)
            {
                _gridModel.SetCellState(x, y, CellState.Moving);
                FallOneStep(x, y, velocity);
            }
        }

        private void SpawnNewCell(int x, int y)
        {
            _gridModel.RefillCellAtTop(x);
            CellData spawnedCell = _gridModel.GetCell(x, y);
            _gridView.CreateCell(x, y, spawnedCell);
        }
        private void FallOneStep(int x, int y, float velocity)
        {
            int targetY = y - 1;
            CellData cell = _gridModel.GetCell(x, y);
            _gridModel.SetCell(x, targetY, cell.WithState(CellState.Moving));
            _gridModel.ClearCell(x, y);
            float duration = _cascadeService.CalculateFallDuration(velocity);
            float nextVelocity = _cascadeService.CalculateNextVelocity(velocity, duration);
            DOVirtual.DelayedCall(_gridConfig.cascadeDelay, () => CheckAbove(x, y+1, velocity));
            _gridView.MoveCellAnimated(
                x, y,
                x, targetY,
                duration,
                velocity,
                _gridConfig.gravity,
                onComplete: () => OnCellLanded(x, targetY, nextVelocity)
            );
        }
        private void OnCellLanded(int x, int y, float nextVelocity)
        {
            if (_cascadeService.CanCellFall(x, y))
            {
                FallOneStep(x, y, nextVelocity);
            }
            else
            {
                _gridModel.SetCellState(x, y, CellState.Idle);
            }
        }
    }
}