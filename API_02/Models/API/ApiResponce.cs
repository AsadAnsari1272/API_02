using System.Net;

namespace API_02.Models.API
{
    public class ApiResponce
    {
        public object Result { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
        public bool IsSuccess { get; set; }

    }
}
