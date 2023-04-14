using FluentAssertions;
using SFA.DAS.Employer.Profiles.Web.Models;

namespace SFA.DAS.Employer.Profiles.Web.UnitTests.Models;

public class WhenBuildingAddUserDetailsViewModel
{
    [Test]
    public void Then_The_TermsOfUseLink_Is_Correct_For_Production_Environment()
    {
        var actual = new AddUserDetailsModel("prd");
        
        actual.TermsOfUseLink.Should().Be("https://accounts.manage-apprenticeships.service.gov.uk/service/termsAndConditions/overview");
    }
    [Test]
    public void Then_The_Settings_Link_Is_Correct_For_Non_Production_Environment()
    {
        var actual = new AddUserDetailsModel("test");

        actual.TermsOfUseLink.Should().Be("https://accounts.test-eas.apprenticeships.education.gov.uk/service/termsAndConditions/overview");
    }
}