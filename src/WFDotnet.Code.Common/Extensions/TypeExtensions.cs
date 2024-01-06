using System.Reflection;
using WFDotnet.Code.Common.Exceptions;

namespace WFDotnet.Code.Common.Extensions
{
    public static class TypeExtensions
    {
        public static bool CanBeInstantiated(this Type @this)
        {
            if (@this is null)
                throw new TypeIsNotDefinedException();

            return !@this.IsAbstract && !@this.IsValueType && @this.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool HasGenericType(this Type @this)
        {
            if (@this is null)
                throw new TypeIsNotDefinedException();

            return @this.IsGenericType;
        }

        public static bool HasInterface<TInterface>(this object @this)
        {
            var hasInterface = @this.GetType().GetInterface(typeof(TInterface).Name);
            return hasInterface is not null;
        }

        public static TInterface GetInterface<TInterface>(this object @this)
        {
            var hasInterface = @this.HasInterface<TInterface>();

            if (!hasInterface) return default(TInterface);

            return (TInterface)@this;
        }

        public static string GetFullName(this Type type, bool isGenericType)
        {
            if (!isGenericType) return type.FullName;
            var genericPropertyCommaIndex = type.FullName.IndexOf('`');
            return type.FullName.Substring(0, genericPropertyCommaIndex);
        }

        public static bool IsAttribute(this Type type)
        {
            return type.IsSubclassOf(typeof(Attribute));
        }

        public static PropertyInfo[] GetProperties(this object @this)
        {
            return @this.GetType().GetProperties();
        }

        public static bool HasAttribute<TAttribute>(this PropertyInfo propertyInfo)
            where TAttribute : Attribute
        {
            var attribute = propertyInfo.GetCustomAttribute(typeof(TAttribute));
            return attribute is not null;
        }

        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo propertyInfo)
            where TAttribute : Attribute
        {
            var attribute = propertyInfo.GetCustomAttribute(typeof(TAttribute));

            if (attribute is null) return default(TAttribute);

            return (TAttribute)attribute;
        }

        public static bool ConstainsSubType(this Type @this, Type type)
        {
            if (@this.BaseType != null && @this.BaseType == type)
                return true;
            else if (@this.BaseType != null)
                return @this.BaseType.ConstainsSubType(type);

            return false;
        }
    }
}
