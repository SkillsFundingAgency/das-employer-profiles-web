using FluentAssertions;
using SFA.DAS.Employer.Profiles.Web.Models;

namespace SFA.DAS.Employer.Profiles.Web.UnitTests.Models;

public class WhenBuildingChangeSignInDetailsViewModel
{
    [Test]
    public void Then_The_Settings_Link_Is_Correct_For_Production_Environment()
    {
        var actual = new ChangeSignInDetailsViewModel("prd");
        
        actual.SettingsLink.Should().Be("https://home.account.gov.uk/settings");
    }
    [Test]
    public void Then_The_Settings_Link_Is_Correct_For_Non_Production_Environment()
    {
        var actual = new ChangeSignInDetailsViewModel("test");

        actual.SettingsLink.Should().Be("https://home.integration.account.gov.uk/settings");
    }
}