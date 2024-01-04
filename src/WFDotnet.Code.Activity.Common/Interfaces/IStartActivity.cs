namespace WFDotnet.Code.Activity.Common.Interfaces
{
    public interface IStartActivity : IActivity 
    {
        string Name { get; }
        Dictionary<string,string> Arguments { get; set; }
    }
}
