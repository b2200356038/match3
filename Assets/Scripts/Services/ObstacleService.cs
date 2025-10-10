using System.Collections.Generic;
using Game.Core.Data;
using Game.Features.Grid.Model;
using UnityEngine;

namespace Game.Services
{
    public class ObstacleService
    {
        private readonly GridModel _gridModel;
        public ObstacleService(GridModel gridModel)
        {
            _gridModel = gridModel;
        }
        public List<Vector2Int> GetAffectedObstacles(List<Vector2Int> matches)
        {
            HashSet<Vector2Int> affectedObstacles = new HashSet<Vector2Int>();
            foreach (var matchPos in matches)
            {
                var obstacleNeighbors = GetObstacleNeighbors(matchPos.x, matchPos.y);

                foreach (var neighbor in obstacleNeighbors)
                {
                    affectedObstacles.Add(neighbor);
                }
            }
            return new List<Vector2Int>(affectedObstacles);
        }
        private List<Vector2Int> GetObstacleNeighbors(int x, int y)
        {
            List<Vector2Int> obstacles = new List<Vector2Int>();
            if (IsObstacleAt(x, y + 1)) obstacles.Add(new Vector2Int(x, y + 1));
            if (IsObstacleAt(x + 1, y)) obstacles.Add(new Vector2Int(x + 1, y));
            if (IsObstacleAt(x, y - 1)) obstacles.Add(new Vector2Int(x, y - 1));
            if (IsObstacleAt(x - 1, y)) obstacles.Add(new Vector2Int(x - 1, y));

            return obstacles;
        }
        private bool IsObstacleAt(int x, int y)
        {
            if (!_gridModel.IsValidPosition(x, y))
                return false;
            CellData cell = _gridModel.GetCell(x, y);
            return cell.IsObstacle;
        }
    }
}