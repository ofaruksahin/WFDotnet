using WFDotnet.Code.Activity.Common.Attributes;
using WFDotnet.Code.Activity.Common.Interfaces;
using WFDotnet.Code.Activity.Common.Models;

namespace WFDotnet.Code.Activity.Basic
{
    public class GetKeyValueItemActivity : IActivity
    {
        [ActivityInParameter("Name", "","")]
        public string Name { get; set; }
        public StepResult Result { get; set; }

        [ActivityInParameter("Arguments","","")]
        public KeyValueItem Arguments { get; set; }

        [ActivityInParameter("SearchKey","","")]
        public KeyValueItem SearchKey { get; set; }

        [ActivityOutParameter("Value","")]
        public object Value { get; set; }

        public Task OnExecute()
        {
            if(Arguments.Value is KeyValueItem[] arguments)
            {
                var argument = arguments.FirstOrDefault(f => f.Key.Equals(SearchKey.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));

                if (argument is null)
                {
                    Result = new StepFailResult();
                }
                else
                {
                    Value = argument.Value;

                    Result = new StepSuccessResult();
                }
            }
            else
            {
                Result = new StepFailResult();
            }

            return Task.CompletedTask;
        }
    }
}
