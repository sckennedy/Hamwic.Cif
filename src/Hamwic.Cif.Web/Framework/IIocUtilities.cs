using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hamwic.Cif.Web.Framework
{
    public interface IIocUtilities
    {
        void InjectProperties(object target);
        void InjectProperties(object target, Func<PropertyInfo, bool> where);
        T Get<T>();
        object Get(Type type);
        IEnumerable<T> GetAll<T>();
    }
}