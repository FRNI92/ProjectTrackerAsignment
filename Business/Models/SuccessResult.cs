namespace Business.Models;

public class SuccessResult : Result
{
    public SuccessResult(int statusCode, string? message = null)
    {
        Success = true;
        StatusCode = statusCode;
        Message = message;
    }
}
