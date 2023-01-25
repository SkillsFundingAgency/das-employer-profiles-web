using Moq;
using Moq.Protected;

namespace SFA.DAS.Employer.Profiles.Infrastructure.UnitTests.HttpMessageHandlerMock;

public static class MessageHandler
{
    public static Mock<HttpMessageHandler> SetupMessageHandlerMock(HttpResponseMessage response, Uri url, string key, HttpMethod httpMethod)
    {
        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(httpMethod)
                    && c.Headers.Contains("Ocp-Apim-Subscription-Key")
                    && c.Headers.GetValues("Ocp-Apim-Subscription-Key").First().Equals(key)
                    && c.Headers.Contains("X-Version")
                    && c.Headers.GetValues("X-Version").First().Equals("1")
                    && c.RequestUri.Equals(url)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage request, CancellationToken token) => response);
        return httpMessageHandler;
    }
}