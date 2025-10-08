using System.Collections.Generic;
using DG.Tweening;
using Game.Core.Data;
using Game.Features.Grid.Model;
using Game.Features.Grid.View;
using UnityEngine;

namespace Game.Services
{
    public class CascadeService
    {
        private readonly GridModel _gridModel;
        private readonly GridView _gridView;
        private readonly PhysicsService _physicsService;
        private readonly GridConfig _gridConfig;

        public CascadeService(GridModel gridModel, GridView gridView, PhysicsService physicsService, GridConfig gridConfig)
        {
            _gridModel = gridModel;
            _gridView = gridView;
            _physicsService = physicsService;
            _gridConfig = gridConfig;
        }
        public void ProcessCascades(List<Vector2Int> matches)
        {
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
            
            DOVirtual.DelayedCall(_gridConfig.cascadeDelay, () =>
            {
                if (aboveY == _gridModel.VisibleHeight)
                {
                    CellData spawnCell = _gridModel.GetCell(x, aboveY);
                    if (spawnCell.IsEmpty)
                    {
                        SpawnNewCellAtTop(x);
                        FallOneStep(x, aboveY, initialVelocity);
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