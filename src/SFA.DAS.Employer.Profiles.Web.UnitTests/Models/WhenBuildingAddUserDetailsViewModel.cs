using FluentAssertions;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Testing.AutoFixture;

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
    public void Then_The_TermsOfUseLink_Is_Correct_For_Non_Production_Environment()
    {
        var actual = new AddUserDetailsModel("test");

        actual.TermsOfUseLink.Should().Be("https://accounts.test-eas.apprenticeships.education.gov.uk/service/termsAndConditions/overview");
    }

    [Test]
    public void Then_The_RedirectLink_Is_Correct_For_Production_Environment()
    {
        var actual = new AddUserDetailsModel("prd");

        actual.RedirectUrl.Should().Be("https://accounts.manage-apprenticeships.service.gov.uk");
    }

    [Test]
    public void Then_The_RedirectLink_Is_Correct_For_Non_Production_Environment()
    {
        var actual = new AddUserDetailsModel("test");

        actual.RedirectUrl.Should().Be("https://accounts.test-eas.apprenticeships.education.gov.uk");
    }

    [Test, MoqAutoData]
    public void Then_FirstName_Given_ModelStateError_Return_ErrorMessage(
        string env,
        string firstNameError,
        string lastNameError)
    {
        var actual = new AddUserDetailsModel(env)
        {
            ErrorDictionary = new Dictionary<string, string?>
            {
                { nameof(AddUserDetailsModel.FirstName), firstNameError },
                { nameof(AddUserDetailsModel.LastName), lastNameError },
            }
        };

        actual.FirstNameError.Should().NotBeNullOrEmpty();
        actual.LastNameError.Should().NotBeNullOrEmpty();
        actual.FirstNameError.Should().Be(firstNameError);
        actual.LastNameError.Should().Be(lastNameError);
    }
}