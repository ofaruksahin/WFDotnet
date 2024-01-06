using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.Common.DataTypes
{
    public class BooleanDataType : IDataType, INullable, IAttributeValue
    {
        public bool IsNullable { get; set; }

        public Type GetSystemType()
        {
            if (IsNullable)
                return typeof(bool?);
            return typeof(bool);
        }

        public object GetAttributeValue(object value)
        {
            return value.ToString().ToLowerInvariant();
        }
    }
}
