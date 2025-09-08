namespace KnightDesk.Presentation.WPF.DTOs
{
    public class ServerConnectionRequestDTO
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutMs { get; set; } = 3000;
    }

    public class ServerConnectionResponseDTO
    {
        public bool IsReachable { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ResponseTimeMs { get; set; }
    }
}
