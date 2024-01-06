using WFDotnet.Code.Activity.Common.Models;

namespace WFDotnet.Code.Activity.Common.Interfaces
{
    public interface IActivity
    {
        string Name { get; set; }
        StepResult Result { get; set; }
        Task OnExecute();
    }
}
