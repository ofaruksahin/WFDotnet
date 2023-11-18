namespace WFDotnet.Code.DataModel.Exceptions
{
	public class DataModelTypeIsNotDefinedException : Exception
	{
		public DataModelTypeIsNotDefinedException(string name)
			: base ($"{name} data model type is not defined")
		{

		}
	}
}

