using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.Employer.Profiles.Domain.Configuration;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;
using SFA.DAS.Employer.Profiles.Infrastructure.UnitTests.HttpMessageHandlerMock;

namespace SFA.DAS.Employer.Profiles.Infrastructure.UnitTests.ApiClient;

public class WhenCallingGetOnTheApiClient
{
    
    [Test, AutoData]
    public async Task Then_The_Endpoint_Is_Called_With_Authentication_Header_And_Data_Returned(
        List<string> testObject, 
        EmployerProfilesWebConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var configMock = new Mock<IOptions<EmployerProfilesWebConfiguration>>();
        configMock.Setup(x => x.Value).Returns(config);
        var getTestRequest = new GetTestRequest();
        
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(testObject)),
            StatusCode = HttpStatusCode.Accepted
        };
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(config.BaseUrl + getTestRequest.GetUrl), config.Key, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var apiClient = new Api.ApiClient(client, configMock.Object);

        //Act
        var actual = await apiClient.Get<List<string>>(getTestRequest);
        
        //Assert
        actual.Body.Should().BeEquivalentTo(testObject);
    }
    
    [Test, AutoData]
    public async Task Then_If_It_Is_Not_Successful_An_Error_Is_Returned(
        EmployerProfilesWebConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var configMock = new Mock<IOptions<EmployerProfilesWebConfiguration>>();
        configMock.Setup(x => x.Value).Returns(config);
        var getTestRequest = new GetTestRequest();
        var response = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = HttpStatusCode.BadRequest
        };
        
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response,new Uri(config.BaseUrl + getTestRequest.GetUrl), config.Key, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var apiClient = new Api.ApiClient(client, configMock.Object);
        
        //Act Assert
        var actual = await apiClient.Get<List<string>>(getTestRequest);
        actual.StatusCode.Equals(HttpStatusCode.BadRequest);
        actual.Body.Should().BeNull();

    }
    
    [Test, AutoData]
    public async Task Then_If_It_Is_Not_Found_Default_Is_Returned(
        EmployerProfilesWebConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var configMock = new Mock<IOptions<EmployerProfilesWebConfiguration>>();
        configMock.Setup(x => x.Value).Returns(config);
        var getTestRequest = new GetTestRequest();
        var response = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = HttpStatusCode.NotFound
        };
        
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(config.BaseUrl + getTestRequest.GetUrl), config.Key, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var apiClient = new Api.ApiClient(client, configMock.Object);
        
        //Act Assert
        var actual = await apiClient.Get<List<string>>(getTestRequest);

        actual.Body.Should().BeNull();
        actual.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private class GetTestRequest : IGetApiRequest
    {
        public string GetUrl => $"/test-url/get";
    }
}