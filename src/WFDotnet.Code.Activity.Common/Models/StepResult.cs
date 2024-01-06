namespace WFDotnet.Code.Activity.Common.Models
{
    public class StepResult
    {
        public bool IsSuccess { get; set; }
    }

    public class StepSuccessResult : StepResult
    {
        public StepSuccessResult()
        {
            IsSuccess = true;
        }
    }

    public class StepFailResult : StepResult
    {
        public StepFailResult()
        {
            IsSuccess = false;
        }
    }
}
