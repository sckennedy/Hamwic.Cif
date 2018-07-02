using System;

namespace Hamwic.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SortOrderAttribute : Attribute
    {
        public SortOrderAttribute(int position)
        {
            Position = position;
        }

        public int Position { get; }
    }
}