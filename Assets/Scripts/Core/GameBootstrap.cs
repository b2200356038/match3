using Game.Core.Data;
using Game.Features.Grid.Model;
using Game.Features.Grid.Presenter;
using Game.Features.Grid.View;
using Game.Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Configuration")] 
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private CellConfig _cellConfig;

        [Header("View Reference")] 
        [SerializeField] private GridView _gridView;

        private GridModel _gridModel;
        private GridPresenter _gridPresenter;
        private MatchService _matchService;
        private PhysicsService _physicsService;
        private CascadeService _cascadeService;
        private CellFactory _cellFactory;
        private RefillService _refillService;
        private CellPoolService _cellPoolService;

        
        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (_gridConfig == null)
            {
                return;
            }

            if (_gridView == null)
            {
                return;
            }
            _cellFactory = new CellFactory(_cellConfig);
            _gridModel = new GridModel(_gridConfig, _cellFactory);
            _cellPoolService = new CellPoolService(_cellConfig);
            _matchService = new MatchService(_gridModel, _gridConfig.MinMatchCount);
            _physicsService = new PhysicsService(_gridConfig.gravity, _gridConfig.CellSize);
            _refillService = new RefillService(_gridModel, _gridConfig);
            _cascadeService = new CascadeService(_gridModel, _gridView, _physicsService, _gridConfig);
            _gridView.Initialize(_gridConfig.Width, _gridConfig.Height, _gridConfig.CellSize, _cellPoolService);
            _gridPresenter = new GridPresenter(_gridModel, _gridView, _matchService, _cascadeService, _gridConfig);
            _gridPresenter.Initialize();
            _gridPresenter.StartGame();
        }

        private void OnDestroy()
        {
            if (_gridPresenter != null)
            {
                _gridPresenter.Dispose();
            }
        }
    }
}