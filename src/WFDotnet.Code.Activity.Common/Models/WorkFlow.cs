using WFDotnet.Code.Activity.Common.Interfaces;

namespace WFDotnet.Code.Activity.Common.Models
{
    public class WorkFlow
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public WorkFlowStep[] Steps { get; set; }

        public WorkFlow()
        {
            Steps = new WorkFlowStep[0];
        }
    }

    public class WorkFlowStep
    {
        public IActivity Activity { get; set; }
        public WorkFlowStep[] Steps { get; set; }
    }
}
