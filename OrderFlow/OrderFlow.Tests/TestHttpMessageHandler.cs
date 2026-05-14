using System.Net;

namespace OrderFlow.Tests;

public class TestHttpMessageHandler : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    public int CallCount { get; private set; }

    public Func<HttpRequestMessage, HttpResponseMessage>? Handler { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;

        CallCount++;

        return Task.FromResult(
            Handler?.Invoke(request)
            ?? new HttpResponseMessage(HttpStatusCode.OK));
    }
}