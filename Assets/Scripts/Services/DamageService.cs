using System.Collections.Generic;
using Game.Core.Data;
using Game.Features.Grid.Model;
using UnityEngine;

namespace Game.Services
{
    public class DamageService
    {
        private readonly GridModel _gridModel;
        
        public DamageService(GridModel gridModel)
        {
            _gridModel = gridModel;
        }
        
        public List<Vector2Int> ApplyMatchDamage(List<Vector2Int> matches)
        {
            HashSet<Vector2Int> damagedPositions = CollectDamageTargets(matches);
            return ApplyDamage(damagedPositions);
        }
        
        private HashSet<Vector2Int> CollectDamageTargets(List<Vector2Int> matches)
        {
            HashSet<Vector2Int> targets = new HashSet<Vector2Int>();
            
            foreach (var pos in matches)
            {
                targets.Add(pos);
            }
            foreach (var matchPos in matches)
            {
                var neighbors = GetAdjacentCells(matchPos.x, matchPos.y);
                
                foreach (var neighbor in neighbors)
                {
                    CellData cell = _gridModel.GetCell(neighbor.x, neighbor.y);
                    if (!cell.IsEmpty && cell.IsBreakable)
                    {
                        targets.Add(neighbor);
                    }
                }
            }
            
            return targets;
        }
        private List<Vector2Int> ApplyDamage(HashSet<Vector2Int> positions)
        {
            List<Vector2Int> destroyedCells = new List<Vector2Int>();
            
            foreach (var pos in positions)
            {
                CellData cell = _gridModel.GetCell(pos.x, pos.y);
                
                if (cell.IsEmpty || !cell.IsBreakable)
                    continue;
                CellData damagedCell = cell.TakeDamage(1);
                
                if (damagedCell.Health <= 0)
                {
                    destroyedCells.Add(pos);
                }
                else
                {
                    _gridModel.SetCell(pos.x, pos.y, damagedCell);
                }
            }
            
            return destroyedCells;
        }
        
        private List<Vector2Int> GetAdjacentCells(int x, int y)
        {
            List<Vector2Int> adjacent = new List<Vector2Int>();

            if (IsObstacleAt(x, y + 1)) adjacent.Add(new Vector2Int(x, y + 1));
            if (IsObstacleAt(x + 1, y)) adjacent.Add(new Vector2Int(x + 1, y));
            if (IsObstacleAt(x, y - 1)) adjacent.Add(new Vector2Int(x, y - 1));
            if (IsObstacleAt(x - 1, y)) adjacent.Add(new Vector2Int(x - 1, y));
    
            return adjacent;
        }

        private bool IsObstacleAt(int x, int y)
        {
            if (!_gridModel.IsValidPosition(x, y))
            {
                return false;
            }
    
            CellData cell = _gridModel.GetCell(x, y);
            return cell.IsObstacle;
        }
    }
}