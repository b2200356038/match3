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
            CellData startCell = _gridModel.GetCell(startX, startY);
            List<Vector2Int> matches = new List<Vector2Int>();
            if (startCell.IsEmpty || !startCell.CanClick)
            {
                return new List<Vector2Int>();
            }
    
            bool[,] visited = new bool[_gridModel.Width, _gridModel.Height];
            FindMatchesDFS(startX, startY, startCell.Type, matches, visited);
            if (matches.Count < _minMatchCount)
            {
                return new List<Vector2Int>();
            }
    
            return matches;
        }
    
        private void FindMatchesDFS(int x, int y, CellType targetType, List<Vector2Int> matches, bool[,] visited)
        {
            if (!_gridModel.IsValidPosition(x, y))
                return;
            if (visited[x, y])
                return;
            CellData cell = _gridModel.GetCell(x, y);
            if (cell.Type != targetType || cell.IsEmpty || cell.State != CellState.Idle)
                return;
    
            visited[x, y] = true;
            matches.Add(new Vector2Int(x, y));
            FindMatchesDFS(x, y + 1, targetType, matches, visited);
            FindMatchesDFS(x+1, y, targetType, matches, visited);
            FindMatchesDFS(x, y - 1, targetType, matches, visited);
            FindMatchesDFS(x-1, y , targetType, matches, visited);
        }
        public Dictionary<int, List<Vector2Int>> FindAllMatches()
        {
            var allMatches = new Dictionary<int, List<Vector2Int>>();
            var globalVisited = new bool[_gridModel.Width, _gridModel.Height];
            int matchId=0;
            for (int x = 0; x < _gridModel.Width; x++)
            {
                for (int y = 0; y < _gridModel.Height; y++)
                {
                    if (globalVisited[x,y])
                    {
                        continue;
                    }

                    CellData cell = _gridModel.GetCell(x, y);
                    if (cell.IsEmpty || !cell.CanClick)
                    {
                        continue;
                    }

                    List<Vector2Int> matches = new List<Vector2Int>();
                    bool[,] visited = new bool[_gridModel.Width, _gridModel.Height];
                    FindMatchesDFS(x,y,cell.Type,matches,visited);
                    if (matches.Count<_minMatchCount)
                    {
                        allMatches[matchId] = matches;
                        matchId++;
                        foreach (var pos in matches)
                        {
                            globalVisited[pos.x, pos.y] = true;
                        }
                    }
                }
            }
            return allMatches;
        }

        public bool HasMatchAt(int x, int y)
        {
            return FindMatches(x, y).Count >= _minMatchCount;
        }

        public bool HasAnyMatches()
        {
            return FindAllMatches().Count > 0;
        }
    }
}