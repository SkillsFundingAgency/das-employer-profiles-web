using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.Employer.Profiles.Web.Controllers;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Employer.Profiles.Web.UnitTests.Controllers;

public class ServiceControllerTests
{
    [Test, MoqAutoData]
    public void Then_The_View_Is_Returned_With_Model(
        [Frozen] Mock<IConfiguration> configuration, 
        [Greedy] ServiceController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");

        var actual = controller.SignedOut() as ViewResult;

        actual.Should().NotBeNull();
        var actualModel = actual?.Model as SignedOutViewModel;
        Assert.AreEqual("https://accounts.test-eas.apprenticeships.education.gov.uk",actualModel?.ServiceLink);
    }
}