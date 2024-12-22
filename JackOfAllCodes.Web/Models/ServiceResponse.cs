using JackOfAllCodes.Web.Models.Domain;

namespace JackOfAllCodes.Web.Models
{
    public class ServiceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }

        public ServiceResponse() { }

        public ServiceResponse(bool success, string message = null, string userId = null)
        {
            Success = success;
            Message = message;
            UserId = userId;
        }
    }
}
