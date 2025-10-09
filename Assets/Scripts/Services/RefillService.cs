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
    }
}