using System.Collections.Generic;
using Game.Core.Data;
using UnityEngine;

namespace Game.Services
{
    public class PowerUpService
    {
        private CellConfig _cellConfig;
        public PowerUpService(CellConfig cellConfig)
        {
            _cellConfig = cellConfig;
        }
        public bool TryGetPowerUp(List<Vector2Int> matches, out PowerUpType powerUpType)
        {
            if (matches.Count < 4)
            {
                powerUpType = default;
                return false;
            }
            powerUpType = DeterminePowerUpType(matches);
            return true;
        }

        private PowerUpType DeterminePowerUpType(List<Vector2Int> matches)
        {
            if (matches.Count >= 7)
            {
                return PowerUpType.Bomb;
            }

            int random = Random.Range(0, 2);
            return random == 0 ? PowerUpType.RowRocket : PowerUpType.ColumnRocket;
        }
    }
}
