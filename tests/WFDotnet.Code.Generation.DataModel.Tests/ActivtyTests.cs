using WFDotnet.Code.Activity.Basic;
using WFDotnet.Code.Activity.Common.Models;
using WFDotnet.Code.Generation.Activity;

namespace WFDotnet.Code.Generation.DataModel.Tests
{
    public class ActivtyTests
    {
        [Fact]
        public void Test()
        {
            WorkFlow workFlow = new WorkFlow
            {
                Name = "FirstWorkFlow",
                Description = "Oluşturduğum ilk iş akışı",
                Activities = new List<WorkFlowActivity>
                {
                    new WorkFlowActivity
                    {
                         Activity = new StartActivity()
                         {
                              Arguments = new Dictionary<string, string>
                              {
                                  {"name","Ömer Faruk" }
                              }
                         },
                    },
                    new WorkFlowActivity
                    {
                        Activity = new EndActivity()
                    }
                }
            };

            var code = new ActivityCodeGenerator().Generate(workFlow);
            Console.WriteLine(code.IsSuccess);
        }
    }
}


public class FirstWorkFlow
{
    public async Task<object> Execute()
    {
        var start = new StartActivity();
        start.Arguments.Add("item1", "item2");
        start.Arguments.Add("item2", "item2");
        await start.OnExecute();

        return null;
    }
}