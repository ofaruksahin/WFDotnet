namespace WFDotnet.Code.DataModel.Models
{
    public class Model
    {
        public string Name { get; set; }
        public Property[] Properties { get; set; }

        public Model()
        {
            Properties = new Property[0];
        }
    }
}

