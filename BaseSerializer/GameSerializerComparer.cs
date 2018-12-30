using System.Collections.Generic;
using BaseSerializer.Implementations;

namespace BaseSerializer
{
    public class GameSerializerComparer : IComparer<IGameSerializer>
    {
        public static readonly GameSerializerComparer Instance = new GameSerializerComparer();

        public int Compare(IGameSerializer x, IGameSerializer y)
        {
            if (x == null)
            {
                return y == null ? 0 : 1;
            }

            if (y == null)
            {
                return -1;
            }

            return x.Order - y.Order;
        }
    }
}
