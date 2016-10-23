using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmostPDP11.VM.Extentions
{
    static class UtilExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            return list.Select((item, index) => new { index, item })
                        .GroupBy(x => x.index % 2 == 0 ? x.index : x.index - 1)
                        .Select(x => x.Select(y => y.item));
        }
    }
}
