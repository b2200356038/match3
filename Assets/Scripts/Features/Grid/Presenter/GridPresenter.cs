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
        private readonly DamageService _damageService;
        private readonly PowerUpService _powerUpService;
        private readonly GridConfig _gridConfig;

        public GridPresenter(GridModel gridModel, GridView gridView, MatchService matchService,
            CascadeService cascadeService, DamageService damageService, PowerUpService powerUpService,
            GridConfig gridConfig)
        {
            _gridModel = gridModel;
            _gridView = gridView;
            _matchService = matchService;
            _cascadeService = cascadeService;
            _damageService = damageService;
            _powerUpService = powerUpService;
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

            CellData clickedSlot = _gridModel.GetCell(x, y);

            if (clickedSlot.IsPowerUp)
            {
                return;
            }

            if (clickedSlot.IsCube)
            {
                var matches = _matchService.FindMatches(x, y);

                if (matches.Count < _gridConfig.MinMatchCount)
                {
                    return;
                }

                ProcessMatches(matches);
            }
        }

        private void ProcessMatches(List<Vector2Int> matches)
        {
            List<Vector2Int> destroyedCells = _damageService.ApplyMatchDamage(matches);

            if (_powerUpService.TryGetPowerUp(matches, out PowerUpType powerUpType))
            {
                foreach (var pos in destroyedCells)
                {
                    _gridModel.ClearCell(pos.x, pos.y);

                    if (matches.Contains(pos))
                        continue;
                    _gridView.RemoveCell(pos.x, pos.y);
                }

                destroyedCells.Remove(matches[0]);
                CellData powerUp = _gridModel.SpawnPowerUp(powerUpType, matches[0]);
                _gridView.CreatePowerUp(matches, powerUp, onComplete: () =>
                {
                    _gridModel.SetCellFallable(powerUp.ToggleCanFall());
                    _cascadeService.ProcessCascades();
                });
            }
            else
            {
                foreach (var pos in destroyedCells)
                {
                    _gridView.RemoveCell(pos.x, pos.y);
                    _gridModel.ClearCell(pos.x, pos.y);
                }

                _cascadeService.ProcessCascades();
            }
        }
    }
}