using WFDotnet.Code.Activity.Common.Interfaces;
using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.Activity.Common.Models
{
    public class ValueItem : IValueItem
    {
        public IDataType DataType { get; set; }
        public bool FromActivity { get; set; }
        public bool FromValue { get; set; }
        public object Value { get; set; }
    }
}
