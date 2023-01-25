using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiRequests;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Employer.Profiles.Application.UnitTests.Employer;

public class WhenGettingUserAccounts
{
    [Test, MoqAutoData]
    public async Task Then_The_Request_Is_Made_And_Accounts_Returned(
        string userId,
        string email,
        ApiResponse<GetUserAccountsResponse> apiResponse,
        [Frozen] Mock<IApiClient> apiClient,
        EmployerAccountService service)
    {
        //Arrange
        var request = new GetUserAccountsRequest(userId, email);
        apiClient.Setup(x =>
                x.Get<GetUserAccountsResponse>(
                    It.Is<GetUserAccountsRequest>(c => c.GetUrl.Equals(request.GetUrl))))
            .ReturnsAsync(apiResponse);
            
        //Act
        var actual = await service.GetUserAccounts(userId, email);
            
        //Assert
        actual.EmployerAccounts.Should().BeEquivalentTo(apiResponse.Body.UserAccounts);
    }
}