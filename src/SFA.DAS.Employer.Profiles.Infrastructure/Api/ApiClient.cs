using Microsoft.Extensions.Options;
using SFA.DAS.Employer.Profiles.Domain.Configuration;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;
using System.Text;
using System.Text.Json;

namespace SFA.DAS.Employer.Profiles.Infrastructure.Api;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly EmployerProfilesWebConfiguration _config;

    public ApiClient (HttpClient httpClient, IOptions<EmployerProfilesWebConfiguration> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
        _httpClient.BaseAddress = new Uri(config.Value.BaseUrl);
    }
    
    public async Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
        AddAuthenticationHeader(requestMessage);
        
        var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        return await ProcessResponse<TResponse>(response);
    }

    public async Task<ApiResponse<TResponse>> Put<TResponse>(IPutApiRequest request)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, request.PutUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json"),
            VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
        };
        
        AddAuthenticationHeader(requestMessage);

        var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        return await ProcessResponse<TResponse>(response);
    }

    private void AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
    {
        httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _config.Key);
        httpRequestMessage.Headers.Add("X-Version", "1");
    }

    private static async Task<ApiResponse<TResponse>> ProcessResponse<TResponse>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        var errorContent = "";
        var responseBody = (TResponse)default!;
        
        if(!response.IsSuccessStatusCode)
        {
            errorContent = json;
        }
        else
        {
            responseBody = JsonSerializer.Deserialize<TResponse>(json);
        }

        var apiResponse = new ApiResponse<TResponse>(responseBody, response.StatusCode, errorContent);
        
        return apiResponse;
    }
}
