using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.Employer.Profiles.Web.Controllers;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Employer.Profiles.Web.UnitTests.Controllers;

public class UserControllerTests
{
    [Test, MoqAutoData]
    public void Then_The_View_Is_Returned_With_Model(
        [Frozen] Mock<IConfiguration> configuration, 
        [Greedy] UserController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");

        var actual = controller.ChangeSignInDetails() as ViewResult;

        actual.Should().NotBeNull();
        var actualModel = actual?.Model as ChangeSignInDetailsViewModel;
        Assert.AreEqual("https://home.integration.account.gov.uk/settings",actualModel?.SettingsLink);
    }
}