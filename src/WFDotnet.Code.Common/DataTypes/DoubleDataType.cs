using WFDotnet.Code.Common.DataTypes.Interfaces;

namespace WFDotnet.Code.Common.DataTypes
{
    public class DoubleDataType : IDataType, INullable
    {
        public bool IsNullable { get; set; }

        public Type GetSystemType()
        {
            if (IsNullable)
                return typeof(double?);
            return typeof(double);
        }
    }
}
