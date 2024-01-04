using WFDotnet.Code.Activity.Common.Enums;

namespace WFDotnet.Code.Activity.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ActivityInParameterAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public InParameterComponentType Component { get; private set; }

        public ActivityInParameterAttribute(
            string name,
            string description, 
            string category,
            InParameterComponentType component)
        {
            Name = name;
            Description = description;
            Category = category;
            Component = component; 
        }
    }
}
