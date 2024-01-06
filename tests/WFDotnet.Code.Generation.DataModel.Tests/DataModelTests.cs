using WFDotnet.Code.Activity.Basic;
using WFDotnet.Code.Activity.Common;
using WFDotnet.Code.Activity.Common.Models;
using WFDotnet.Code.Common.Constants;
using WFDotnet.Code.Common.Serialization;
using WFDotnet.Code.DataModel.Models;

namespace WFDotnet.Code.Generation.DataModel.Tests;

public class DataModelTests
{
    [Fact]
    public void GenerateClass()
    {
        var model = new Model();
        model.Name = "User";

        var properties = new List<Property>();

        properties.Add(new Property
        {
            Name = "Name",
            TypeFinder = new StringDataType()
        });

        model.Properties = properties.ToArray();

        var json = model.ToSerialize();

        var codeGenerationResult = new DataModelCodeGenerator().Generate(model);
    }

    [Fact]
    public void GenerateWorkFlow()
    {
        WorkFlow workFlow = new WorkFlow
        {
            Name = "FirstWorkflow",
            Description = "My First Work Flow",
            Steps = new List<WorkFlowStep>
            {
                new WorkFlowStep
                {
                    Activity = new StartActivity()
                    {
                         Name = "start",
                         Arguments = new List<KeyValueItem>
                         {
                             new KeyValueItem
                             {
                                 Key = "Name",
                                 FromValue = true,
                                 Value = "Ömer Faruk",
                                 DataType = new StringDataType()
                             }
                         }.ToArray()
                    }
                },
                new WorkFlowStep
                {
                    Activity = new GetKeyValueItemActivity
                    {
                         Name = "GetName",
                         Arguments = new KeyValueItem
                         {
                              FromActivity = true,
                              Value = "start.Arguments",
                         },
                         SearchKey = new KeyValueItem
                         {
                             FromActivity = false,
                             Value = "Name"
                         }
                    }
                },
                new WorkFlowStep
                {
                    Activity = new EndActivity()
                    {
                       
                    }
                }
            }.ToArray()
        };

        var q = workFlow.ToSerialize();

        new WorkFlowRunner().Run(workFlow).GetAwaiter().GetResult();
    }
}