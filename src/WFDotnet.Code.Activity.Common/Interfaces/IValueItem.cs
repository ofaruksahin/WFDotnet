using System.Text.Json.Serialization;
using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.Activity.Common.Interfaces
{
    public interface IValueItem
    {
        public IDataType DataType { get; set; }
        public bool FromActivity { get; set; }
        public bool FromValue { get; set; }
        public object Value { get; set; }

        [JsonIgnore]
        public Type ValueType
        {
            get => DataType.GetSystemType();
        }
    }
}
