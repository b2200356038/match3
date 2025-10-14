using System.Collections.Generic;
using Game.Core;
using Game.Core.Data;
using UnityEngine;

namespace Game.Services
{
    public class MatchService
    {
        private readonly int _minMatchCount;

        public MatchService(int minMatchCount = 2)
        {
            _minMatchCount = minMatchCount;
        }

        public List<Vector2Int> FindMatches(Cell[,] grid, int width, int height, int startX, int startY)
        {
            List<Vector2Int> matches = new List<Vector2Int>();

            var startCell = grid[startX, startY];
            if (startCell == null || startCell is not CubeCell startCube)
                return matches;

            if (!startCell.CanMatch || startCell.State != CellState.Idle)
                return matches;

            bool[,] visited = new bool[width, height];
            FindMatchesDFS(grid, width, height, startX, startY, startCube.ColorType, matches, visited);

            if (matches.Count < _minMatchCount)
                return new List<Vector2Int>();

            return matches;
        }

        private void FindMatchesDFS(Cell[,] grid, int width, int height, int x, int y, 
            CubeType targetColor, List<Vector2Int> matches, bool[,] visited)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            if (visited[x, y])
                return;

            var cell = grid[x, y];
            if (cell == null || cell is not CubeCell cube)
                return;

            if (cube.ColorType != targetColor || cell.State != CellState.Idle)
                return;

            visited[x, y] = true;
            matches.Add(new Vector2Int(x, y));

            FindMatchesDFS(grid, width, height, x, y + 1, targetColor, matches, visited);
            FindMatchesDFS(grid, width, height, x + 1, y, targetColor, matches, visited);
            FindMatchesDFS(grid, width, height, x, y - 1, targetColor, matches, visited);
            FindMatchesDFS(grid, width, height, x - 1, y, targetColor, matches, visited);
        }
    }
}