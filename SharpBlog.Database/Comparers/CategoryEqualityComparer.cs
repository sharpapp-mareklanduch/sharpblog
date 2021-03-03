using SharpBlog.Database.Models;
using System;
using System.Collections.Generic;

namespace SharpBlog.Database.Comparers
{
    public class CategoryEqualityComparer : IEqualityComparer<Category>
    {
        public bool Equals(Category x, Category y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || x == null)
                return false;
            else if (string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase))
                return true;
            else
                return false;
        }

        public int GetHashCode(Category obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
