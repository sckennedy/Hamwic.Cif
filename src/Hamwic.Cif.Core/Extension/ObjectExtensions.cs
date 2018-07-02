using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace Hamwic.Core.Extension
{
    public static class ObjectExtensions
    {
        public static readonly string[] PropertyDelimiter = {"."};

        public static T WithId<T>(this T instance, object id)
        {
            typeof(T)
                .GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)
                .SetValue(instance, id, null);

            return instance;
        }

        public static T WithProperty<T>(this T instance, Expression<Func<T, object>> action, object value)
        {
            var expression = GetMemberInfo(action);
            typeof(T)
                .GetProperty(expression.Member.Name, BindingFlags.Instance | BindingFlags.Public)
                .SetValue(instance, value, null);
            return instance;
        }

        public static T WithProperty<T>(this T instance, string propertyPath, object value)
        {
            var objectAndProperty = instance.GetProperty(propertyPath);
            objectAndProperty.Item2.SetValue(objectAndProperty.Item1,
                Convert.ChangeType(value, objectAndProperty.Item2.PropertyType), null);
            return instance;
        }

        public static T CopyPropertiesFrom<T>(this T target, object source)
        {
            CopyAllProperties(source, target);
            return target;
        }

        //public static T WithProperty<T>(this T instance, string propertyPath, IReadRepository readRepository,
        //    string typeName, string typeId, string idValue)
        //{
        //    var type = Type.GetType(typeName);
        //    var genericGetMethod = typeof(IReadRepository).GetMethod("Get");
        //    var closedGetMethod = genericGetMethod.MakeGenericMethod(type);

        //    var idType = Type.GetType(typeId);
        //    var id = Convert.ChangeType(TypeDescriptor.GetConverter(idType).ConvertFromInvariantString(idValue),
        //        idType);
        //    var result = closedGetMethod.Invoke(readRepository, new[] {id});

        //    instance.WithProperty(propertyPath, result);
        //    return instance;
        //}

        public static void CopyAllProperties(this object source, object target)
        {
            var sourceProps = source.GetType().GetProperties().Where(x => !x.HasAttribute<OmitFromCopyProperties>());
            var targetProps = target.GetType().GetProperties().Where(x => !x.HasAttribute<OmitFromCopyProperties>()).ToList();

            foreach (var pi in sourceProps)
            {
                var targetProp = targetProps.FirstOrDefault(x => x.Name == pi.Name);
                if (targetProp == null || !targetProp.CanWrite) continue;
                try
                {
                    targetProp.SetValue(target, pi.GetValue(source, null), null);

                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException("Failed to copy property from source to target", targetProp.Name, e);
                }
            }
        }

        public static Tuple<object, PropertyInfo> GetProperty(this object instance, string propertyPath)
        {
            var obj = instance;
            while (true)
            {
                Argument.IsNullOrEmpty(propertyPath, nameof(propertyPath));

                if (!propertyPath.Contains("."))
                    return new Tuple<object, PropertyInfo>(obj, obj.GetType().GetProperty(propertyPath));

                var propParts = propertyPath.Split(PropertyDelimiter, StringSplitOptions.RemoveEmptyEntries);

                obj = obj.GetType().GetProperty(propParts[0]).GetValue(instance, null);
                propertyPath = string.Join(".", propParts, 1, propParts.Length - 1);
            }
        }

        public static object GetPropertyValue(this object instance, string propertyPath)
        {
            var objectAndProperty = instance.GetProperty(propertyPath);
            return objectAndProperty.Item2.GetValue(objectAndProperty.Item1, null);
        }

        public static T WithProperties<T>(this T instance, IDictionary<string, object> propertyValues)
        {
            if (propertyValues == null || propertyValues.Count == 0)
                return instance;

            var type = instance.GetType();
            foreach (var property in propertyValues)
            {
                var propertyInfo = type.GetProperty(property.Key);
                if (propertyInfo == null)
                    continue;

                propertyInfo.SetValue(instance, Convert.ChangeType(property.Value, propertyInfo.PropertyType));
            }

            return instance;
        }

        public static T WithProtectedProperty<T>(this T instance, Expression<Func<T, object>> action, object value)
        {
            var expression = GetMemberInfo(action);
            typeof(T)
                .GetProperty(expression.Member.Name)
                .SetValue(instance, value, null);
            return instance;
        }

        public static T WithPrivateMember<T>(this T instance, string memberName, object value)
        {
            typeof(T)
                .GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(instance, value);
            return instance;
        }

        public static string PropertyName<T>(this T instance, Expression<Func<T, object>> action)
        {
            return GetMemberInfo(action).Member.Name;
        }


        public static MemberExpression GetMemberInfo(Expression method)
        {
            var lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            switch (lambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpr = ((UnaryExpression) lambda.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpr = lambda.Body as MemberExpression;
                    break;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }

        /// <summary>
        ///     Populates an object with default values based on their type. Primarily
        ///     used for testing
        /// </summary>
        /// <param name="instance">The object on which to set the value</param>
        /// <param name="propsToIgnore"></param>
        public static T WithDefaultPropertyValues<T>(this T instance, params string[] propsToIgnore)
        {
            foreach (var property in typeof(T).GetProperties().Where(x =>
                x.CanWrite && (propsToIgnore == null ||
                               !propsToIgnore.Any(p => string.Equals(p, x.Name, StringComparison.OrdinalIgnoreCase)))))
            {
                var propertyType = property.PropertyType;

                if (propertyType == typeof(string))
                {
                    property.SetValue(instance, property.Name, null);
                }
                else if (propertyType == typeof(Guid))
                {
                    property.SetValue(instance, Guid.NewGuid(), null);
                }
                else if (propertyType == typeof(bool))
                {
                    property.SetValue(instance, true, null);
                }
                else if (propertyType == typeof(int) ||
                         propertyType == typeof(short) ||
                         propertyType == typeof(long))
                {
                    property.SetValue(instance, Convert.ChangeType(123, propertyType), null);
                }
                else if (propertyType == typeof(decimal))
                {
                    property.SetValue(instance, (decimal) 1234.56, null);
                }
                else if (propertyType == typeof(double))
                {
                    property.SetValue(instance, 1234.56, null);
                }
                else if (propertyType == typeof(DateTime) ||
                         propertyType == typeof(DateTime?))
                {
                    property.SetValue(instance, DateTime.Today, null);
                }
                else if (propertyType.Name == "Nullable`1")
                {
                    var nullType = Nullable.GetUnderlyingType(propertyType);

                    if (nullType == typeof(float) ||
                        nullType == typeof(double) ||
                        nullType == typeof(decimal))
                        {
                            if (property.Name == "Lat" || property.Name == "Long")
                            {
                                property.SetValue(instance, Convert.ChangeType("0.12", nullType), null);
                            } else {
                                property.SetValue(instance, Convert.ChangeType("1234.56", nullType), null);
                            }
                        }


                    if (nullType == typeof(int) ||
                        nullType == typeof(short) ||
                        nullType == typeof(long))
                        property.SetValue(instance, Convert.ChangeType("123", nullType), null);
                }
            }
            return instance;
        }

        public static string ToXmlString(this object instance)
        {
            if (instance == null)
                return string.Empty;

            var serializer = new XmlSerializer(instance.GetType());
            var xmlSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false,
                NewLineHandling = NewLineHandling.None
            };

            // remove the default xsi and xsd namespaces
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var writer = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(writer, xmlSettings))
            {
                serializer.Serialize(xmlWriter, instance, ns);
                xmlWriter.Flush();
                return writer.ToString();
            }
        }

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", "source");

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
                return default(T);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T) formatter.Deserialize(stream);
            }
        }

        public static object Clone(object source)
        {
            Argument.IsNull(source, "source");

            if (!source.GetType().IsSerializable)
                throw new ArgumentException("The source type must be serializable.", "source");

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }

        public static T FromDynamic<T>(dynamic expando)
        {
            var entity = Activator.CreateInstance<T>();

            //ExpandoObject implements dictionary
            var properties = expando as IDictionary<string, object>;

            if (properties == null)
                return entity;

            var type = entity.GetType();
            foreach (var entry in properties)
            {
                var propertyInfo = type.GetProperty(entry.Key);
                if (propertyInfo != null)
                    propertyInfo.SetValue(entity, entry.Value, null);
            }
            return entity;
        }

        public static string ToStringLinq(this object o)
        {
            return o.GetType().FullName
                   + Environment.NewLine
                   + string.Join(Environment.NewLine, from p in o.GetType().GetProperties()
                       select string.Format("{0}{1}{2}", p.Name, ':', p.GetValue(o, null)));
        }
    }
}