using Newtonsoft.Json;
using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.DataModel.Models
{
    public class Property
    {
        public string Name { get; set; }
        public IDataType TypeFinder { get; set; }

        public Attribute[] Attributes { get; set; }

        public Property()
        {
            Attributes = new Attribute[0];
        }

        [JsonIgnore]
        public Type PropertyType
        {
            get => TypeFinder.GetSystemType();
        }
    }
}

