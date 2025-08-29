using Newtonsoft.Json;

namespace KnightDesk.Core.Application.DTOs
{
    public class GeneralResponseDTO<T>
    {
        public GeneralResponseDTO()
        {
            Errors = new List<ResponseError>();
        }
        [JsonProperty("code")]
        public int Code { get; set; } = (int)RESPONSE_CODE.OK;
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("errors")]
        public IList<ResponseError> Errors { get; set; }
        [JsonProperty("data")]
        public T Data { get; set; }
        [JsonProperty("meta")]
        public ResponseMeta Meta { get; set; }
        public void AddSingleError(string errorMsg, int code)
        {
            Errors.Add(new ResponseError { Message = errorMsg, Code = code });
        }
        public bool IsValid => (Errors?.Count ?? 0) == 0;

        public GeneralResponseDTO(T data)
        {
            Data = data;
        }
        public GeneralResponseDTO(int code, string? message)
        {
            Code = code;
            Message = message!;
        }
    }

    public class ResponseError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string? Message { get; set; }
        [JsonProperty("field")]
        public string? Field { get; set; }
    }
    public class ResponseMeta
    {
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("page")]
        public int Page { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
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
