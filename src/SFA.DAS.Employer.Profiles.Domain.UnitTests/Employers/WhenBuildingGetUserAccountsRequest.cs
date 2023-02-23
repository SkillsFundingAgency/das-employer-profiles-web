using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiRequests;

namespace SFA.DAS.Employer.Profiles.Domain.UnitTests.Employers;

public class WhenBuildingGetUserAccountsRequest
{
    [Test, AutoData]
    public void Then_The_Request_Is_Correctly_Built_And_Returned(string email, string userId)
    {
        email = email + "!@£ $" + email;
        var actual = new GetUserAccountsRequest(userId, email);

        actual.GetUrl.Should().Be($"accountusers/{userId}/accounts?email={HttpUtility.UrlEncode(email)}");
    }
}