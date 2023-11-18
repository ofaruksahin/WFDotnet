using WFDotnet.Code.Generation.Common.Constants;
using WFDotnet.Code.Generation.Common.Models;

namespace WFDotnet.Code.Generation.Common
{
    public abstract class WFCodeGenerator<TActivity>
		where TActivity : class
	{
		protected virtual List<string> GetUsings()
		{
			return DefaultNamespace.GetDefaultNamespaces();
		}

		public abstract WFCodeGenerationResult Generate(TActivity instance);
	}
}

