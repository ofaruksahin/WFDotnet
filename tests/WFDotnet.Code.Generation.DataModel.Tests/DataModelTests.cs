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
}