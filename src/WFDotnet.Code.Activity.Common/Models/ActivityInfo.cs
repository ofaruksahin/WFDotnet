using System.Reflection;
using WFDotnet.Code.Activity.Common.Attributes;
using WFDotnet.Code.Activity.Common.Interfaces;
using WFDotnet.Code.Common.Extensions;

namespace WFDotnet.Code.Activity.Common.Models
{
    public class ActivityInfo
    {
        public bool IsStartActivity { get; private set; }
        public bool IsEndActivity { get; private set; }
        public bool IsInitializable { get; private set; }
        public bool HasConstructor { get; private set; }
        public IConstructor Constructor { get; private set; }
        public IDictionary<string, object> InParameters { get; private set; }
        public IDictionary<string,object> OutParameters { get; private set; }

        public IActivity Activity { get; private set; }

        public ActivityInfo(IActivity activity)
        {
            Activity = activity;

            SetIsStartActivity();
            SetIsEndActivity();
            SetHasConstructor();
            SetInParameters();
            SetOutParameters();
        }

        private void SetIsStartActivity()
        {
            IsStartActivity = Activity.HasInterface<IStartActivity>();
        }

        private void SetIsEndActivity()
        {
            IsEndActivity = Activity.HasInterface<IEndActivity>();
        }

        private void SetHasConstructor()
        {
            HasConstructor = Activity.HasInterface<IConstructor>();

            if (!HasConstructor) return;

            Constructor = Activity.GetInterface<IConstructor>();
        }

        private void SetInParameters()
        {
            InParameters = new Dictionary<string, object>();

            var properties = Activity.GetProperties();

            foreach (var property in properties)
            {
                if (!property.HasAttribute<ActivityInParameterAttribute>()) continue;

                var propertyName = property.Name;
                var propertyValue = property.GetValue(Activity);

                InParameters.Add(propertyName, propertyValue);
            }
        }

        private void SetOutParameters()
        {

        }

        public void SetProperty(string name, object value)
        {
            var properties = Activity.GetProperties();

            foreach (var item in properties)
            {
                var hasInParamAttribute = item.HasAttribute<ActivityInParameterAttribute>();

                if (hasInParamAttribute)
                {
                    var inParamAttribute = item.GetAttribute<ActivityInParameterAttribute>();

                    if(inParamAttribute.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetProperty(item, value);
                    }
                }
                else
                {
                    var hasOutParamAttribute = item.HasAttribute<ActivityOutParameterAttribute>();

                    if (hasOutParamAttribute)
                    {
                        var outParamAttribute = item.GetAttribute<ActivityOutParameterAttribute>();

                        if(outParamAttribute.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            SetProperty(item, value);
                        }
                    }
                }
            }
        }

        private void SetProperty(PropertyInfo propertyInfo, object value)
        {
            if (propertyInfo.PropertyType.ConstainsSubType(typeof(ValueItem)))
            {
                var newInstance = (ValueItem)Activator.CreateInstance(propertyInfo.PropertyType);

                newInstance.FromActivity = true;
                newInstance.Value = value;

                propertyInfo.SetValue(Activity, newInstance);
            }
            else
            {
                propertyInfo.SetValue(Activity, value);
            }
        }
    }
}
