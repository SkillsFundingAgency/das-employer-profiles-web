using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiRequests;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Employer.Profiles.Application.UnitTests.Employer;

public class WhenUpdatingUserAccount
{
    [Test, MoqAutoData]
    public async Task Then_The_Request_Is_Made_And_Account_Updated(
        string userId,
        UpsertAccountRequest upsertRequest,
        ApiResponse<PutUserAccountResponse> apiResponse,
        [Frozen] Mock<IApiClient> apiClient,
        EmployerAccountService service)
    {
        //Arrange
        var request = new UpsertUserApiRequest(userId, upsertRequest);
        apiClient.Setup(x =>
                x.Put<PutUserAccountResponse>(
                    It.Is<UpsertUserApiRequest>(c => c.PutUrl.Equals(request.PutUrl)
                    && c.Data.Equals(request.Data)
                    )))
            .ReturnsAsync(apiResponse);

        //Act
        var actual = await service.UpsertUserAccount(userId, upsertRequest);
            
        //Assert
        actual.Email.Should().BeEquivalentTo(apiResponse.Body.Email);
        actual.FirstName.Should().BeEquivalentTo(apiResponse.Body.FirstName);
        actual.LastName.Should().BeEquivalentTo(apiResponse.Body.LastName);
        actual.UserId.Should().BeEquivalentTo(apiResponse.Body.UserId);
    }
}