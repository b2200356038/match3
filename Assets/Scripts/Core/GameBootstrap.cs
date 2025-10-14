using Game.Core.Data;
using UnityEngine;

namespace Game.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private CellConfig _cellConfig;

        [Header("References")]
        [SerializeField] private Board _board;

        private void Start()
        {
            if (_gridConfig == null || _cellConfig == null || _board == null)
            {
                return;
            }
            _board.Initialize(_gridConfig, _cellConfig);
        }
    }
}