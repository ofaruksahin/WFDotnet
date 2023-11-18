namespace WFDotnet.Code.Generation.DataModel.Extensions
{
    internal static class TypeExtension
	{
		public static string GetFullName(this Type type, bool isGenericType)
		{
			if (!isGenericType) return type.FullName;
            var genericPropertyCommaIndex = type.FullName.IndexOf('`');
            return type.FullName.Substring(0, genericPropertyCommaIndex);
        }
    }
}

