using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.Common.Constants
{
    public class StringDataType : IDataType, INullable, IAttributeValue
    {
        public bool IsNullable { get; set; }

        public object GetAttributeValue(object value)
        {
            return $"\"{value}\"";
        }

        public Type GetSystemType()
        {
            if (IsNullable)
                return Nullable.GetUnderlyingType(typeof(string));
            return typeof(string);
        }
    }
}
