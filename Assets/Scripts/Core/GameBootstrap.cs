using Game.Core.Data;
using Game.Features.Grid.Model;
using Game.Features.Grid.Presenter;
using Game.Features.Grid.View;
using Game.Services;
using UnityEngine;

namespace Game.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private GridConfig _gridConfig;

        [Header("View Reference")] [SerializeField]
        private GridView _gridView;

        private GridModel _gridModel;
        private GridPresenter _gridPresenter;
        private MatchService _matchService;
        private PhysicsService _physicsService;

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

            _gridModel = new GridModel(_gridConfig);
            _matchService = new MatchService(_gridModel, _gridConfig.MinMatchCount);
            _physicsService = new PhysicsService(20f, _gridConfig.CellSize);
            _gridView.Initialize(_gridConfig.Width, _gridConfig.Height, _gridConfig.CellSize);
            _gridPresenter = new GridPresenter(
                _gridModel,
                _gridView,
                _matchService,
                _physicsService,
                _gridConfig
            );
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