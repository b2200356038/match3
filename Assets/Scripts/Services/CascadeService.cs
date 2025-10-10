using System;
using Game.Core.Data;
using Game.Features.Grid.Model;
using UnityEngine;

namespace Game.Services
{
    public class CascadeService
    {
        public event Action<Vector2Int, Vector2Int, float> OnCellShouldFall;
        public event Action<Vector2Int, CellData> OnCellShouldSpawn;
        private readonly GridModel _gridModel;
        public CascadeService(GridModel gridModel)
        {
            _gridModel = gridModel;
        }
    }
}