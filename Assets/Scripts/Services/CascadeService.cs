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

        public void ProcessCascades()
        {
            for (int x = 0; x < _gridModel.Width; x++)
            {
                for (int y = 0; y < _gridModel.Height; y++)
                {
                    CellData cell = _gridModel.GetCell(x, y);

                    if (cell.IsEmpty)
                    {
                        CheckAbove(x, y, 0);
                    }
                }
            }
        }

        private void CheckAbove(int x, int y, float initialVelocity)
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
                    CellData spawnSlot = _gridModel.GetCell(x, aboveY);
                    if (spawnSlot.IsEmpty)
                    {
                        SpawnNewCellAtTop(x);
                        FallOneStep(x, aboveY, initialVelocity);
                    }
                }
                
                CellData aboveSlot = _gridModel.GetCell(x, aboveY);
                if (aboveSlot.IsFallable && aboveSlot.State != CellState.Moving)
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
            CellData slot = _gridModel.GetCell(x, y);
            
            if (slot.IsEmpty)
            {
                return;
            }
            
            int targetY = y - 1;
            
            if (targetY < 0)
            {
                _gridModel.SetCellState(x, y, CellState.Idle);
                return;
            }
            
            CellData targetSlot = _gridModel.GetCell(x, targetY);
            if (!targetSlot.IsEmpty)
            {
                _gridModel.SetCellState(x, y, CellState.Idle);
                return;
            }
            
            _gridModel.SetCell(x, targetY, slot.WithState(CellState.Moving));
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
            _gridModel.RefillCellAtTop(x);
            int spawnY = _gridModel.Height - 1;
            CellData spawnedSlot = _gridModel.GetCell(x, spawnY);
            _gridView.CreateCell(x, spawnY, spawnedSlot);
        }
    }
}