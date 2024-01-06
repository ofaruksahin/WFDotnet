using WFDotnet.Code.Activity.Common.Attributes;
using WFDotnet.Code.Activity.Common.Interfaces;
using WFDotnet.Code.Activity.Common.Models;

namespace WFDotnet.Code.Activity.Basic
{
    public class StartActivity : IStartActivity, IConstructor
    {
        [ActivityInParameter("Arguments","","")]
        public KeyValueItem[] Arguments { get; set; }

        [ActivityInParameter("Name","","")]
        public string Name { get; set; }

        public StepResult Result { get; set; }

        public StartActivity()
        {
            Name = "start";
            Arguments = new KeyValueItem[0];
        }

        public Task OnExecute()
        {
            Result = new StepSuccessResult();

            return Task.CompletedTask;
        }
    }
}
