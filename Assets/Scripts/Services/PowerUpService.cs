using System.Collections.Generic;
using Game.Core.Data;
using Game.Features.Grid.Model;
using UnityEngine;

namespace Game.Services
{
    public class PowerUpService
    {
        private readonly CellConfig _cellConfig;
        private readonly GridModel _gridModel;
        public PowerUpService(GridModel gridModel, CellConfig cellConfig)
        {
            _cellConfig = cellConfig;
            _gridModel = gridModel;
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
        public void ActivatePowerUp(int x, int y)
        {
            CellData cellData = _gridModel.GetCell(x, y);
            if (!cellData.IsPowerUp)
            {
                return;
            }

            switch (cellData.PowerUpType)
            {
                case PowerUpType.RowRocket:
                    StartRowRocket(x, y);
                    break;
                case PowerUpType.ColumnRocket:
                    StartColumnRocket(x, y);
                    break;
                case PowerUpType.Bomb:
                    StartBomb(x, y);
                    break;
            }
        }

        private void StartBomb(int x, int y)
        {
            _gridModel.ClearCell(x,y);
            
        }

        private void StartRowRocket(int x, int y)
        {
        }

        private void StartColumnRocket(int x, int y)
        {
        }
    }
}
