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
}
