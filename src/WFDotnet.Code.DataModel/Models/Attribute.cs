using Newtonsoft.Json;
using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.DataModel.Models
{
    public class Attribute
    {
        public IDataType TypeFinder { get; set; }
        public AttributeValue[] Values { get; set; }

        [JsonIgnore]
        public Type AttributeType
        {
            get => TypeFinder.GetSystemType();
        }

        public Attribute()
        {
            Values = new AttributeValue[0];
        }
    }
}
