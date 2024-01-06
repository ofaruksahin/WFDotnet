using WFDotnet.Code.Activity.Common.Interfaces;
using WFDotnet.Code.Activity.Common.Models;

namespace WFDotnet.Code.Activity.Basic
{
    public class EndActivity : IEndActivity
    {
        public string Name { get; set; }
        public StepResult Result { get; set; }

        public EndActivity()
        {
            Name = "end";
        }

        public Task OnExecute()
        {
            Result = new StepSuccessResult();

            return Task.CompletedTask;
        }
    }
}
