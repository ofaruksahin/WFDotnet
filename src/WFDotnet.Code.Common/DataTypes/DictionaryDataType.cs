using WFDotnet.Code.Common.DataTypes.Interfaces;
using WFDotnet.Code.Common.Exceptions;

namespace WFDotnet.Code.Common.DataTypes
{
    public class DictionaryDataType : IDataType
    {
        public IDataType KeyType { get; set; }
        public IDataType ValueType { get; set; }

        public Type GetSystemType()
        {
            if (KeyType is null || ValueType is null)
                throw new TypeIsNotDefinedException();

            return typeof(Dictionary<,>).MakeGenericType(KeyType.GetSystemType(), ValueType.GetSystemType());
        }
    }
}
