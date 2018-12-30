using System.Collections.Generic;
using PatrickSachs.Serializer.Implementations;

namespace PatrickSachs.Serializer
{
    public class BaseSerializerComparer : IComparer<IBaseSerializer>
    {
        public static readonly BaseSerializerComparer Instance = new BaseSerializerComparer();

        public int Compare(IBaseSerializer x, IBaseSerializer y)
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
