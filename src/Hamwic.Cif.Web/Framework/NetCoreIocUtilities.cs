using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hamwic.Cif.Core;
using Hamwic.Cif.Core.CoreEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hamwic.Cif.Web.Framework
{
    public class NetCoreIocUtilities : IIocUtilities
    {
        private static readonly Dictionary<Type, IEnumerable<PropertyInfo>> CachedInjectableProperties;
        private static readonly object SyncRoot = new object();

        static NetCoreIocUtilities()
        {
            CachedInjectableProperties = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        }

        public NetCoreIocUtilities(IHttpContextAccessor context)
        {
            Context = context;
        }

        public IHttpContextAccessor Context {get; }

        public void InjectProperties(object target)
        {
            InjectProperties(target, pi => true);
        }

        public void InjectProperties(object target, Func<PropertyInfo, bool> where)
        {
            Argument.IsNull(target, "target");

            var type = target.GetType();
            var injectableProperties = GetInjectableProperties(type).Where(x => x.PropertyType.FullName.StartsWith("Hamwic"));
            foreach (var property in injectableProperties)
            {
                if (where != null && !where(property))
                    continue;

                try
                {
                    var value = Context.HttpContext.RequestServices.GetService(property.PropertyType);
                    property.SetValue(target, value, null);
                }
                catch (Exception ex)
                {
                    var message = string.Format("Error setting property {0} on type {1}. {2}",
                        property.Name,
                        type.FullName,
                        ex.Message);

                    throw new InvalidOperationException(message, ex);
                }
            }
        }

        public T Get<T>()
        {
            return Context.HttpContext.RequestServices.GetService<T>();
        }

        public object Get(Type type)
        {
            return Context.HttpContext.RequestServices.GetService(type);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return Context.HttpContext.RequestServices.GetServices<T>();
        }

        private static IEnumerable<PropertyInfo> GetInjectableProperties(Type type)
        {
            lock (SyncRoot)
            {
                if (CachedInjectableProperties.ContainsKey(type))
                    return CachedInjectableProperties[type];
            }

            lock (SyncRoot)
            {
                if (CachedInjectableProperties.ContainsKey(type))
                    return CachedInjectableProperties[type];

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(property => property.CanWrite)
                    .ToList();

                CachedInjectableProperties[type] = properties;
                return properties;

            }
        }
    }
}