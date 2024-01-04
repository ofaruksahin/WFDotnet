using System.Diagnostics;
using WFDotnet.Code.Activity.Common.Interfaces;

namespace WFDotnet.Code.Activity.Basic
{
    public class StartActivity : IStartActivity
    {
        public string Name => "start";

        public Dictionary<string, string> Arguments { get; set; }

        public StartActivity()
        {
            Arguments = new Dictionary<string, string>();
        }

        public Task OnExecute()
        {
            if(Debugger.IsAttached)
                Console.WriteLine("Start activity on executed");

            return Task.CompletedTask;
        }
    }
}
