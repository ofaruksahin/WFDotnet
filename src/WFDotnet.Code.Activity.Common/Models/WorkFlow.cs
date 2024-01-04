using WFDotnet.Code.Activity.Common.Interfaces;

namespace WFDotnet.Code.Activity.Common.Models
{
    public class WorkFlow
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WorkFlowActivity> Activities { get; set; }
        public WorkFlow()
        {
            Activities = new List<WorkFlowActivity>();
        }
    }

    public class WorkFlowActivity
    {
        public IActivity Activity { get; set; }
        public List<WorkFlowActivity> SubActivities { get; set; }
    }
}
