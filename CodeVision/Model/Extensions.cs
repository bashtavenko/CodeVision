using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.Model
{
    public static class Extensions
    {
        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> list, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber cannot be less than 1");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "PageSize cannot be less than 1");
            }
            
            return pageNumber == 1 ? list.Take(pageSize) : list.Skip((pageNumber - 1)*pageSize).Take(pageSize);
        }
    }
}
