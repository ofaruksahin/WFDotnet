using WFDotnet.Code.DataModel.Exceptions;

namespace WFDotnet.Code.DataModel
{
    public class ModelPropertyInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public List<ModelPropertyAttributeInfo> Attributes { get; set; }

        public ModelPropertyInfo()
        {
            Attributes = new List<ModelPropertyAttributeInfo>();
        }

        public bool CanBeInstantiated()
        {
            if (Type is null)
                throw new DataModelTypeIsNotDefinedException(Name ?? nameof(ModelPropertyInfo));

            return !Type.IsAbstract && !Type.IsValueType && Type.GetConstructor(Type.EmptyTypes) != null;
        }

        public bool HasGenericType()
        {
            if (Type is null)
                throw new DataModelTypeIsNotDefinedException(Name ?? nameof(ModelPropertyInfo));

            return Type.IsGenericType;
        }
    }
}

