namespace WFDotnet.Code.Generation.Common.Constants
{
    public class DefaultNamespace
	{
		public string Namespace { get; private set; }

		public DefaultNamespace(string @namespace)
		{
			Namespace = @namespace;
		}

		public static DefaultNamespace System = new DefaultNamespace("System");
		public static DefaultNamespace Collections = new DefaultNamespace("System.Collections");
		public static DefaultNamespace CollectionsGeneric = new DefaultNamespace("System.Collections.Generic");

		public static List<string> GetDefaultNamespaces()
		{
			return new List<string>
			{
				DefaultNamespace.System.Namespace
			};
		}
	}
}

