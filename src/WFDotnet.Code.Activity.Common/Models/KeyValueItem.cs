using WFDotnet.Code.Activity.Common.Interfaces;

namespace WFDotnet.Code.Activity.Common.Models
{
    public class KeyValueItem : ValueItem, IKeyValueItem
    {
        public string Key { get; set; }
    }
}
