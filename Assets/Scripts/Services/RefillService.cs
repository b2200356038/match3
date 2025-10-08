using Game.Core.Data;
using Game.Features.Grid.Model;
using UnityEngine;

namespace Game.Services
{
    public class RefillService
    {
        private readonly GridModel _gridModel;
        private readonly GridConfig _gridConfig;

        public RefillService(GridModel gridModel, GridConfig gridConfig)
        {
            _gridModel = gridModel;
            _gridConfig = gridConfig;
        }

        public CellType GetRandomCellType()
        {
            int random = Random.Range(0, _gridConfig.ColorCount);
            
            return random switch
            {
                0 => CellType.Red,
                1 => CellType.Blue,
                2 => CellType.Green,
                3 => CellType.Yellow,
                _ => CellType.Red
            };
        }
    }
}