namespace WFDotnet.Code.Activity.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActivityAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string CategoryName { get; private set; }

        public ActivityAttribute(
            string name,
            string description,
            string categoryName)
        {
            Name = name;
            Description = description;
            CategoryName = categoryName;
        }
    }
}
