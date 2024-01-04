using System.Diagnostics;
using WFDotnet.Code.Activity.Common.Interfaces;

namespace WFDotnet.Code.Activity.Basic
{
    public class EndActivity : IEndActivity
    {
        public string Name => "end";

        public object Result { get; set; } = null;

        public Task OnExecute()
        {
            if(Debugger.IsAttached)
                Console.WriteLine("End activty on executed");

            return Task.CompletedTask;
        }
    }
}
