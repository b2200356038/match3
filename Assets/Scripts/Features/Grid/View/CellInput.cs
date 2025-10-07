using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Features.Grid.View
{
    public class CellInput: MonoBehaviour, IPointerClickHandler
    {
        private int _x;
        private int _y;
        private GridView _gridView;

        public void Initialize(int x, int y, GridView gridView)
        {
            _x = x;
            _y = y;
            _gridView = gridView;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _gridView.HandleCellClick(_x,_y);
        }
    }
}