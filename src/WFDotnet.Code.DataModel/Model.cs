namespace WFDotnet.Code.DataModel
{
	public class Model
	{
		public string Name { get; set; }
		public List<ModelPropertyInfo> Properties { get; set; }

		public Model()
		{
			Properties = new List<ModelPropertyInfo>();
		}
	}
}

