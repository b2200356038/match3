using System;
using UnityEngine;

namespace Game.Core.Data
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Game/Grid Config")]
    public class GridConfig : ScriptableObject
    {
        
        [Header("Grid Dimensions")] [SerializeField]
        private int width = 8;

        [SerializeField] private int height = 8;

        [Header("Cell Settings")] [SerializeField]
        private float cellSize = 1f;

        [SerializeField] private int colorCount = 4;

        [Header("Match Rules")] [SerializeField]
        private int minMatchCount = 2;

        [Header("Game Rules")] [SerializeField]
        private int startingMoves = 20;

        [SerializeField] private int targetMatches = 10;
        [Header("Animation")]

        [SerializeField] public float gravity = 20f;
        [SerializeField] public float cascadeDelay = 0.02f;
        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;
        public int ColorCount => colorCount;
        public int MinMatchCount => minMatchCount;
        public int StartingMoves => startingMoves;
        public int TargetMatches => targetMatches;

        private void OnValidate()
        {
            width = Mathf.Clamp(width, 4, 16);
            height = Mathf.Clamp(height, 4, 16);
            cellSize = Mathf.Clamp(cellSize, 0.5f, 2f);
            colorCount = Mathf.Clamp(colorCount, 2, 4);
            minMatchCount = Mathf.Clamp(minMatchCount, 2, 5);
            startingMoves = Mathf.Max(1, startingMoves);
            targetMatches = Mathf.Max(1, targetMatches);
        }
    }
}