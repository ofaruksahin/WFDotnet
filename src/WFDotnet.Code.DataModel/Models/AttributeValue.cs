using Newtonsoft.Json;
using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.DataModel.Models
{
    public class AttributeValue
    {
        public string Key { get; set; }
        public IDataType TypeFinder { get; set; }
        public object Value { get; set; }

        [JsonIgnore]
        public Type AttributeValueType
        {
            get => TypeFinder.GetSystemType();
        }
    }
}
