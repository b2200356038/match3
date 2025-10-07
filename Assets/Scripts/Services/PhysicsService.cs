using UnityEngine;

namespace Game.Services
{
    public class PhysicsService
    {
        private readonly float _gravity;
        private readonly float _cellSize;

        public PhysicsService(float gravity = 20f, float cellSize = 1f)
        {
            _gravity = gravity;
            _cellSize = cellSize;
            Debug.Log(($"Physics created: gravity:{_gravity}, cell size:{_cellSize} "));
        }

        public float CalculateFallDuration(float initialVelocity)
        {
            float distance = _cellSize;
            float duration = SolveTimeForDistance(distance, initialVelocity);
  
            return duration;
        }

        public float CalculateVelocity(float initialVelocity, float stepDuration)
        {
            return initialVelocity + stepDuration * _gravity;
        }

        private float SolveTimeForDistance(float distance, float initialVelocity)
        {
            float a = 0.5f * _gravity;
            float b = initialVelocity;
            float c = -distance;

            float discriminant = (b * b) - (4 * a * c);
            if (discriminant < 0)
            {
                return 0.1f;
            }

            float sqrtDisc = Mathf.Sqrt(discriminant);
            float t1 = (-b + sqrtDisc) / (2 * a);
            float t2 = (-b - sqrtDisc) / (2 * a);
            return Mathf.Max(t1, t2);
        }
    }
}