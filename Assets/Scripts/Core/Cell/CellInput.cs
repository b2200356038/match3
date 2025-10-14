using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Core
{
    public class CellInput : MonoBehaviour, IPointerDownHandler
    {
        private int _x;
        private int _y;
        private Board _board;
        public void Initialize(int x, int y, Board board)
        {
            _x = x;
            _y = y;
            _board = board;
            enabled = true;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_board != null)
            {
                _board.HandleCellClick(_x, _y);
            }
        }
        public void Disable()
        {
            enabled = false;
        }
    }
}