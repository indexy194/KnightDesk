namespace KnightDesk.Presentation.WPF.DTOs
{
    public class GeneralResponseDTO<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public GeneralResponseDTO()
        {
            
        }
        public GeneralResponseDTO(int code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

    }
    public enum RESPONSE_CODE
    {
        OK = 200,
        Created = 201,
        NoContent = 204,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        InternalServerError = 500
    }
}
