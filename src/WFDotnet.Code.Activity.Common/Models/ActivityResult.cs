namespace WFDotnet.Code.Activity.Common.Models
{
    public abstract class ActivityResult
    {
        public bool IsSuccess { get; protected set; }
        public string ErrorMessage { get; protected set; }
    }

    public class ActivityOkResult : ActivityResult
    {
        public ActivityOkResult()
        {
            IsSuccess = true;
        }
    }

    public class  ActivityFailResult : ActivityResult 
    {
        public ActivityFailResult(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }
    }
}
