using WFDotnet.Code.Activity.Common.Models;

namespace WFDotnet.Code.Activity.Common.Interfaces
{
    public interface IHasSupportOutputParameter
    {
        string Name { get; }

        ActivityResult Result { get; }
    }
}
