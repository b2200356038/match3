using UnityEngine;

namespace Game.Core.Data
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Game/GridConfig")]
    public class GridConfig : ScriptableObject
    {
        [Header("Grid Dimensions")]
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;

        [Header("Cell Settings")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private int colorCount = 4;

        [Header("Special Cells")]
        [SerializeField] private int obstacleCount = 5;
        [SerializeField] private int cubeCount = 10;

        [Header("Match Rules")]
        [SerializeField] private int minMatchCount = 2;

        [Header("Game Rules")]
        [SerializeField] private int startingMoves = 20;
        [SerializeField] private int targetMatches = 10;

        [Header("Animation")]
        [SerializeField] public float gravity = 20f;
        [SerializeField] public float cascadeDelay = 0.02f;
        [SerializeField] public float maxSpeed=15f;

        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;
        public int ColorCount => colorCount;
        public int ObstacleCount => obstacleCount;
        public int CubeCount => cubeCount;
        public int MinMatchCount => minMatchCount;
        public int StartingMoves => startingMoves;
        public int TargetMatches => targetMatches;
    }
}