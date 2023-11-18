namespace WFDotnet.Code.Generation.Common.Models
{
	public class WFCodeGenerationResult
	{
		public bool IsSuccess { get; private set; }
		public string GeneratedCode { get; private set; }
		public string ExceptionMessage { get; private set; }

		public WFCodeGenerationResult()
		{
		}

		public static WFCodeGenerationResult Success(string generatedCode)
		{
			return new WFCodeGenerationResult
			{
				IsSuccess = true,
				GeneratedCode = generatedCode
			};
		}

		public static WFCodeGenerationResult Fail()
		{
			return new WFCodeGenerationResult
			{
				IsSuccess = false
			};
		}

		public static WFCodeGenerationResult Fail(string exceptionMessage)
		{
			return new WFCodeGenerationResult
			{
				IsSuccess = false,
				ExceptionMessage = exceptionMessage
			};
		}
	}
}

