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
        private readonly GridConfig _gridConfig;

        public GridPresenter(GridModel gridModel, GridView gridView, MatchService matchService,
            CascadeService cascadeService, GridConfig gridConfig)
        {
            _gridModel = gridModel;
            _gridView = gridView;
            _matchService = matchService;
            _cascadeService = cascadeService; 
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
            _gridView.CreateGrid(_gridModel.Slots);
        }

        private void HandleCellClick(int x, int y)
        {
            if (!_gridModel.IsSlotClickable(x, y))
            {
                return;
            }
            CellData clickedSlot = _gridModel.GetSlot(x, y);
            if (clickedSlot.IsPowerUp)
            {
                // TODO: PowerUp activation
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
            foreach (var pos in matches)
            {
                _gridModel.ClearSlot(pos.x, pos.y);
                _gridView.RemoveCell(pos.x, pos.y);
            }
            _cascadeService.ProcessCascades(matches);
        }
    }
}