using UnityEngine;

namespace Game.Services
{
    public class PhysicsService
    {
        private float _gravity;
        private float _cellSize;


        public PhysicsService(float gravity = 20f, float cellSize = 1f)
        {
            _gravity = gravity;
            _cellSize = cellSize;
            Debug.Log(($"Physics created: gravity:{_gravity}, cell size:{_cellSize} "));
        }
        
        
    }
}