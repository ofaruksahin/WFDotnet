using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.Common.Constants
{
    public class IntegerDataType : IDataType, INullable
    {
        public bool IsNullable { get; set; }

        public Type GetSystemType()
        {
            if (IsNullable)
                return Nullable.GetUnderlyingType(typeof(int));
            return typeof(int);
        }
    }
}
