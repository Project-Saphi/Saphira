using System.Net;

namespace Saphira.Saphi.Api;

public class ApiResult<T>
{
    public bool Success { get; set; }
    public T? Response { get; init; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}
