using Game.Core.Data;
using Game.Features.Grid.Model;
using UnityEngine;

namespace Game.Services
{
    public class CascadeService
    {
        private readonly GridModel _gridModel;
        private readonly PhysicsService _physicsService;
        private readonly GridConfig _gridConfig;
        public CascadeService(GridModel gridModel, PhysicsService physicsService, GridConfig gridConfig)
        {
            _gridModel = gridModel;
            _physicsService = physicsService;
            _gridConfig = gridConfig;
        }
        public bool CanCellFall(int x, int y)
        {
            if (!_gridModel.IsValidPosition(x, y)) return false;
            CellData cell = _gridModel.GetCell(x, y);
            if (cell.IsEmpty || !cell.CanFall) return false;
            if (y == 0) return false;
            CellData below = _gridModel.GetCell(x, y - 1);
            return below.IsEmpty;
        }
        public float CalculateFallDuration(float currentVelocity)
        {
            return _physicsService.CalculateFallDuration(currentVelocity);
        }
        public float CalculateNextVelocity(float currentVelocity, float duration)
        {
            return _physicsService.CalculateVelocity(currentVelocity, duration);
        }
    }
}