using System.Collections.Generic;
using Game.Core.Data;
using Game.Features.Grid.Model;
using UnityEngine;

namespace Game.Services
{
    public class MatchService
    {
        private readonly GridModel _gridModel;
        private readonly int _minMatchCount;
    
        public MatchService(GridModel gridModel, int minMatchCount = 2)
        {
            _gridModel = gridModel;
            _minMatchCount = minMatchCount;
        }

        public List<Vector2Int> FindMatches(int startX, int startY)
        {
            CellData startSlot = _gridModel.GetCell(startX, startY);
            List<Vector2Int> matches = new List<Vector2Int>();
            
            if (!startSlot.IsCube || !startSlot.CanMatch)
            {
                return matches;
            }
            bool[,] visited = new bool[_gridModel.Width, _gridModel.Height];
            FindMatchesDFS(startX, startY, startSlot.CubeType, matches, visited);
            if (matches.Count < _minMatchCount)
            {
                return new List<Vector2Int>();
            }
    
            return matches;
        }
    
        private void FindMatchesDFS(int x, int y, CubeType targetType, List<Vector2Int> matches, bool[,] visited)
        {
            if (!_gridModel.IsValidPosition(x, y))
                return;
            
            if (visited[x, y])
                return;
            
            CellData slot = _gridModel.GetCell(x, y);
            
            if (!slot.IsCube || slot.CubeType != targetType || slot.State != CellState.Idle)
                return;
    
            visited[x, y] = true;
            matches.Add(new Vector2Int(x, y));
            
            FindMatchesDFS(x, y + 1, targetType, matches, visited);
            FindMatchesDFS(x + 1, y, targetType, matches, visited);
            FindMatchesDFS(x, y - 1, targetType, matches, visited);
            FindMatchesDFS(x - 1, y, targetType, matches, visited);
        }

        public bool HasMatchAt(int x, int y)
        {
            return FindMatches(x, y).Count > 0;
        }
    }
}