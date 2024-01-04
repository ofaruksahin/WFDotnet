namespace WFDotnet.Code.Activity.Common.Interfaces
{
    public interface IEndActivity : IActivity
    {
        string Name { get; }
        object Result { get; set; }
    }
}
