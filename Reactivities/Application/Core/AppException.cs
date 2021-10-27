namespace Application.Core
{
    public class AppException : System.Exception
    {
        public int Status { get; set; }
        public string ErrorMessage { get; set; }
        public string Details { get; set; }
        
        public AppException(int status, string errorMessage, string details = null)
        {
            Status = status;
            ErrorMessage = errorMessage;
            Details = details;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string details = null) : base(404, "Not found", details)
        {
            
        }
    }
    
    public class BadRequest : AppException
    {
        public BadRequest(string errorMessage, string details = null) : base(500, errorMessage, details)
        {
            
        }
    }

    public class NotAuthorized : AppException
    {
        public NotAuthorized(string errorMessage, string details = null) : base(401, errorMessage, details)
        {
            
        }
    }

    public class AppExceptionDto
    {
        public AppExceptionDto(string message, string details)
        {
            Message = message;
            Details = details;
        }

        public string Message { get; set; }
        public string Details { get; set; }
    }
}