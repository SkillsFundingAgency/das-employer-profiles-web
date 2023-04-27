using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiRequests;
using SFA.DAS.Employer.Profiles.Domain.Models;

namespace SFA.DAS.Employer.Profiles.Domain.UnitTests.Employers;

public class WhenBuildingPutUserAccountRequest
{
    [Test, AutoData]
    public void Then_The_Request_Is_Correctly_Built_And_Returned(string userId, UpsertAccountRequest request)
    {
        var actual = new UpsertUserApiRequest(userId, request);
        var encodedUserId = HttpUtility.UrlEncode(userId);

        actual.PutUrl.Should().Be($"accountusers/{encodedUserId}/upsert-user");
    }
}