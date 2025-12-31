using System.Net;

namespace Saphira.Saphi.Api.Response;

public class SaphiApiResult<T>
{
    public bool Success { get; set; }
    public T? Response { get; init; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}
