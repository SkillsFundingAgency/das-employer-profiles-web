namespace SFA.DAS.Employer.Profiles.Domain.OuterApi;

public interface IApiClient
{
    Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request);
    Task<ApiResponse<TResponse>> Put<TResponse>(IPutApiRequest request);
}