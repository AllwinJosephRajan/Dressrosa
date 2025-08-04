using Dressrosa.Enum;

namespace Dressrosa.Dto
{
    public class ResponseDto<T>
    {
        public T? Data { get; set; }
        public ResponseStatus? Status { get; set; } = ResponseStatus.Success;
        public string? Message { get; set; }
        public object? Metadata { get; set; }
    }
}
