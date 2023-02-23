using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Employer.Profiles.Domain.Employers;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;

namespace SFA.DAS.Employer.Profiles.Domain.UnitTests.Employers;

public class WhenConvertingFromApiResponseToEmployerUserAccounts
{
    [Test, AutoData]
    public void Then_The_Values_Are_Mapped(GetUserAccountsResponse source)
    {
        var actual = (EmployerUserAccounts) source;

        actual.EmployerAccounts.Should().BeEquivalentTo(source.UserAccounts);
    }

    [Test]
    public void Then_If_Null_Then_Empty_Returned()
    {
        var actual = (EmployerUserAccounts) (GetUserAccountsResponse)null;

        actual.EmployerAccounts.Should().BeEmpty();
    }
}