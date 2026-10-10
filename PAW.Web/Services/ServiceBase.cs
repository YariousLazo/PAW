namespace PAW.Web.Services;

public abstract class ServiceBase
{
    protected string BaseUrl { get; set; } = "https://localhost:7038/";

    protected string SetPathUrl(string name) => $"{BaseUrl}{name}";

    /// <summary>
    /// True when the REST call failed because the API answered 404 Not Found.
    /// </summary>
    protected static bool IsNotFound(Exception ex) =>
        ex.InnerException is HttpRequestException { StatusCode: System.Net.HttpStatusCode.NotFound };
}
