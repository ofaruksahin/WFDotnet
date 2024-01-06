using WFDotnet.Code.Activity.Common.Interfaces;
using WFDotnet.Code.Activity.Common.Models;

namespace WFDotnet.Code.Activity.Common
{
    public class WorkFlowRunner
    {
        private IDictionary<string, List<WorkFlowStep>> _parentNodes { get; set; }

        public WorkFlowRunner()
        {
            _parentNodes = new Dictionary<string, List<WorkFlowStep>>();
        }

        public async Task Run(WorkFlow workFlow)
        {
            WorkFlowStep firstStep = workFlow.Steps.FirstOrDefault();
            WorkFlowStep lastStep = workFlow.Steps.LastOrDefault();

            if(firstStep is null || lastStep is null)
            {
                //TODO: Hata Fırlat
            }

            var firstStepActivityInfo = new ActivityInfo(firstStep.Activity);
            var lastStepActivityInfo = new ActivityInfo(lastStep.Activity);

            if(!firstStepActivityInfo.IsStartActivity || !lastStepActivityInfo.IsEndActivity)
            {
                //TODO: Hata Fırlat
            }

            WorkFlowStep parent = null;

            for (int i = 0; i < workFlow.Steps.Length; i++)
            {
                WorkFlowStep current = workFlow.Steps[i];

                await RunActivity(current, parent);

                parent = current;
            }
        }

        private async Task RunActivity(WorkFlowStep current, WorkFlowStep parent = null)
        {
            if (!_parentNodes.ContainsKey(current.Activity.Name))
            {
                if (parent is not null)
                {
                    _parentNodes.Add(current.Activity.Name, new List<WorkFlowStep> { parent });
                }
            }
            else
            {
                if(parent is not null)
                {
                    _parentNodes[current.Activity.Name].Add(parent);
                }
            }

            var currentActivityInfo = new ActivityInfo(current.Activity);

            SetInParameters(currentActivityInfo);

            await current.Activity.OnExecute();

            if(current.Activity.Result.IsSuccess && current.Steps.Any())
            {
                foreach (var step in current.Steps)
                {
                    await RunActivity(step, current);
                }
            }

        }

        private void SetInParameters(ActivityInfo activityInfo)
        {
            if (!_parentNodes.ContainsKey(activityInfo.Activity.Name))
                return;

            var parentNodes = _parentNodes[activityInfo.Activity.Name];

            if (activityInfo.InParameters.Any())
            {
                foreach (var inParam in activityInfo.InParameters)
                {
                    if(inParam.Value is KeyValueItem keyValueItem)
                    {
                        if (!keyValueItem.FromActivity) continue;

                        var value =  keyValueItem.Value.ToString().Split('.');

                        if (value.Length != 2) continue;

                        var activityName = value[0];
                        var argumentName = value[1];

                        var parentNode = parentNodes.FirstOrDefault(f => f.Activity.Name.Equals(activityName, StringComparison.InvariantCultureIgnoreCase));

                        if (parentNode is null) continue;

                        var parentNodeActivityInfo = new ActivityInfo(parentNode.Activity);

                        if (parentNodeActivityInfo.InParameters.ContainsKey(argumentName))
                        {
                            activityInfo.SetProperty(inParam.Key, parentNodeActivityInfo.InParameters[argumentName]);
                        }
                        else if(parentNodeActivityInfo.OutParameters.ContainsKey(argumentName))
                        {
                            activityInfo.SetProperty(inParam.Key, parentNodeActivityInfo.OutParameters[argumentName]);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }
}
