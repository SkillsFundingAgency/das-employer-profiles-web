using FluentAssertions;
using SFA.DAS.Employer.Profiles.Web.Models;

namespace SFA.DAS.Employer.Profiles.Web.UnitTests.Models;

public class WhenBuildingSignedOutViewModel
{
    [TestCase("at","at-eas.apprenticeships.education")]
    [TestCase("test","test-eas.apprenticeships.education")]
    [TestCase("test2","test2-eas.apprenticeships.education")]
    [TestCase("pp", "pp-eas.apprenticeships.education")]
    [TestCase("Mo", "mo-eas.apprenticeships.education")]
    [TestCase("Demo", "demo-eas.apprenticeships.education")]
    [TestCase("prd","manage-apprenticeships.service")]
    public void Then_The_Url_Is_Built_Correctly_For_Each_Environment(string environment, string expectedUrlPart)
    {
        var model = new SignedOutViewModel(environment);

        model.ServiceLink.Should().Be($"https://accounts.{expectedUrlPart}.gov.uk");
    }
}