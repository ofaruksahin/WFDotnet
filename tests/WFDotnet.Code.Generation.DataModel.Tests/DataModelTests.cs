using WFDotnet.Code.DataModel;

namespace WFDotnet.Code.Generation.DataModel.Tests;

public class DataModelTests
{
    [Fact]
    public void ClassGenerationWithOnlySystemTypes()
    {
        var model = new Model();
        model.Name = "User";

        model.Properties.Add(new ModelPropertyInfo
        {
            Name = "Name",
            Type = typeof(string)
        });

        model.Properties.Add(new ModelPropertyInfo
        {
            Name = "Surname",
            Type = typeof(string)
        });

        model.Properties.Add(new ModelPropertyInfo
        {
            Name = "RoleId",
            Type = typeof(int)
        });

        model.Properties.Add(new ModelPropertyInfo
        {
            Name = "CreatedDate",
            Type = typeof(DateTime)
        });

        model.Properties.Add(new ModelPropertyInfo
        {
            Name = "LastModifiedDate",
            Type = typeof(DateTime?)
        });

        model.Properties.Add(new ModelPropertyInfo
        {
            Name = "LastLoginDate",
            Type = typeof(DateTime?)
        });

        var codeGenerationResult = new DataModelCodeGenerator().Generate(model);
    }
}