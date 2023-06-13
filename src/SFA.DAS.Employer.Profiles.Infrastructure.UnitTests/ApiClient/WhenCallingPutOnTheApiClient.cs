using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.Employer.Profiles.Domain.Configuration;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;
using SFA.DAS.Employer.Profiles.Infrastructure.UnitTests.HttpMessageHandlerMock;

namespace SFA.DAS.Employer.Profiles.Infrastructure.UnitTests.ApiClient;

public class WhenCallingPutOnTheApiClient
{
    
    [Test, AutoData]
    public async Task Then_The_Endpoint_Is_Called_With_Authentication_Header_And_Data_Returned(
        UpsertAccountRequest request,
        List<string> testObject, 
        EmployerProfilesWebConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var configMock = new Mock<IOptions<EmployerProfilesWebConfiguration>>();
        configMock.Setup(x => x.Value).Returns(config);
        var putTestRequest = new PutTestRequest(request);
        
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(testObject)),
            StatusCode = HttpStatusCode.Accepted
        };
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(config.BaseUrl + putTestRequest.PutUrl), config.Key, HttpMethod.Put);
        var client = new HttpClient(httpMessageHandler.Object);
        var apiClient = new Api.ApiClient(client, configMock.Object);

        //Act
        var actual = await apiClient.Put<List<string>>(putTestRequest);
        
        //Assert
        actual.Body.Should().BeEquivalentTo(testObject);
    }
    
    [Test, AutoData]
    public async Task Then_If_It_Is_Not_Successful_An_Error_Is_Returned(
        UpsertAccountRequest request,
        EmployerProfilesWebConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var configMock = new Mock<IOptions<EmployerProfilesWebConfiguration>>();
        configMock.Setup(x => x.Value).Returns(config);
        var putTestRequest = new PutTestRequest(request);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = HttpStatusCode.BadRequest
        };

        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(config.BaseUrl + putTestRequest.PutUrl), config.Key, HttpMethod.Put);
        var client = new HttpClient(httpMessageHandler.Object);
        var apiClient = new Api.ApiClient(client, configMock.Object);
        
        //Act Assert
        var actual = await apiClient.Put<List<string>>(putTestRequest);
        actual.StatusCode.Equals(HttpStatusCode.BadRequest);
        actual.Body.Should().BeNull();

    }
    
    [Test, AutoData]
    public async Task Then_If_It_Is_Not_Found_Default_Is_Returned(
        UpsertAccountRequest request,
        EmployerProfilesWebConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var configMock = new Mock<IOptions<EmployerProfilesWebConfiguration>>();
        configMock.Setup(x => x.Value).Returns(config);
        var putTestRequest = new PutTestRequest(request);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = HttpStatusCode.NotFound
        };

        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(config.BaseUrl + putTestRequest.PutUrl), config.Key, HttpMethod.Put);
        var client = new HttpClient(httpMessageHandler.Object);
        var apiClient = new Api.ApiClient(client, configMock.Object);
        
        //Act Assert
        var actual = await apiClient.Put<List<string>>(putTestRequest);

        actual.Body.Should().BeNull();
        actual.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private class PutTestRequest : IPutApiRequest
    {
        public PutTestRequest(UpsertAccountRequest request)
        {
            Data = request;
        }
        public string PutUrl => $"/test-url/put";
        public object Data { get; set; }
    }
}