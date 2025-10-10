using Game.Core.Data;
using UnityEngine;

namespace Game.Services
{
    public class PhysicsService
    {
        private readonly float _gravity;
        private readonly float _cellSize;
        private readonly float _maxVelocity;

        public PhysicsService(GridConfig gridConfig)
        {
            _gravity = gridConfig.gravity;
            _cellSize = gridConfig.CellSize;
            _maxVelocity = gridConfig.maxSpeed;
        }

        public float CalculateFallDuration(float initialVelocity)
        {
            float distance = _cellSize;
            float duration = SolveTimeForDistance(distance, initialVelocity);
            return duration;
        }

        public float CalculateVelocity(float initialVelocity, float stepDuration)
        {
            Debug.Log(initialVelocity);
            if (_maxVelocity<initialVelocity)
            {
                return _maxVelocity;
            }
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