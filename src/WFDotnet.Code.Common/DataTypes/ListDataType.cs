using WFDotnet.Code.Common.DataTypes.Interfaces;
using WFDotnet.Code.Common.Exceptions;

namespace WFDotnet.Code.Common.DataTypes
{
    public class ListDataType : IDataType
    {
        public IDataType IteratorType { get; set; }

        public Type GetSystemType()
        {
            if (IteratorType is null)
                throw new TypeIsNotDefinedException();

            return typeof(List<>).MakeGenericType(IteratorType.GetSystemType());
        }
    }
}
