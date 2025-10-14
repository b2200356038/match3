using Game.Core.Data;

namespace Game.Core
{
    public class CubeCell : Cell
    {
        public CubeType ColorType { get; private set; }

        public void SetColor(CubeType colorType)
        {
            ColorType = colorType;
        }
    }
}