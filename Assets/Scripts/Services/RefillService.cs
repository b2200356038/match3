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

        public CubeType GetRandomCellType()
        {
            int random = Random.Range(0, _gridConfig.ColorCount);
            
            return random switch
            {
                0 => CubeType.Red,
                1 => CubeType.Blue,
                2 => CubeType.Green,
                3 => CubeType.Yellow,
                _ => CubeType.Red
            };
        }
    }
}