namespace WFDotnet.Code.Activity.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ActivityOutParameterAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public ActivityOutParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
