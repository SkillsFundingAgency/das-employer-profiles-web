using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.Configuration;
using SFA.DAS.Employer.Profiles.Domain.Employers;
using SFA.DAS.Employer.Profiles.Web.AppStart;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Employer.Profiles.Web.UnitTests.AppStart;

public class WhenPopulatingAccountClaims
{
    [Test, MoqAutoData]
    public async Task Then_The_Claims_Are_Populated_For_User(
        string nameIdentifier,
        string emailAddress,
        EmployerUserAccounts accountData,
        [Frozen] Mock<IEmployerAccountService> accountService,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IOptions<EmployerProfilesWebConfiguration>> employerProfilesWebConfiguration,
        EmployerAccountPostAuthenticationClaimsHandler handler)
    {
        accountData.IsSuspended = false;
        foreach (var accountDataEmployerAccount in accountData.EmployerAccounts)
        {
            accountDataEmployerAccount.Role = "owner";
        }
        var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, emailAddress);
        accountService.Setup(x => x.GetUserAccounts(nameIdentifier,emailAddress)).ReturnsAsync(accountData);
        
        var actual = await handler.GetClaims(tokenValidatedContext);
        
        accountService.Verify(x=>x.GetUserAccounts(nameIdentifier,emailAddress), Times.Once);
        actual.Should().ContainSingle(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));
        var actualClaimValue = actual.First(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier)).Value;
        actual.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value?.Should().BeNullOrEmpty();
        JsonConvert.SerializeObject(accountData.EmployerAccounts.ToDictionary(k => k.AccountId)).Should().Be(actualClaimValue);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Value.Should().Be(accountData.EmployerUserId);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value.Should().Be(emailAddress);
        actual.First(c => c.Type.Equals(EmployerClaims.GivenName)).Value.Should().Be(accountData.FirstName);
        actual.First(c => c.Type.Equals(EmployerClaims.FamilyName)).Value.Should().Be(accountData.LastName);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier)).Value.Should().Be($"{accountData.FirstName} {accountData.LastName}");
        var accountClaims = actual.Where(c => c.Type.Equals(EmployerClaims.Account)).Select(c => c.Value).ToList();
        accountClaims.Should().BeEquivalentTo(accountData.EmployerAccounts.Select(c => c.AccountId).ToList());
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Claims_Are_Populated_For_User_With_No_Accounts(
        string nameIdentifier,
        string emailAddress,
        EmployerUserAccounts accountData,
        [Frozen] Mock<IEmployerAccountService> accountService,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IOptions<EmployerProfilesWebConfiguration>> employerProfilesWebConfiguration,
        EmployerAccountPostAuthenticationClaimsHandler handler)
    {
        accountData.IsSuspended = false;
        accountData.EmployerAccounts = new List<EmployerUserAccountItem>();
        var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, emailAddress);
        accountService.Setup(x => x.GetUserAccounts(nameIdentifier,emailAddress)).ReturnsAsync(accountData);
        
        var actual = await handler.GetClaims(tokenValidatedContext);
        
        accountService.Verify(x=>x.GetUserAccounts(nameIdentifier,emailAddress), Times.Once);
        actual.Should().ContainSingle(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));
        var actualClaimValue = actual.First(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier)).Value;
        actual.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value?.Should().BeNullOrEmpty();
        JsonConvert.SerializeObject(accountData.EmployerAccounts.ToDictionary(k => k.AccountId)).Should().Be(actualClaimValue);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Value.Should().Be(accountData.EmployerUserId);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value.Should().Be(emailAddress);
        actual.First(c => c.Type.Equals(EmployerClaims.GivenName)).Value.Should().Be(accountData.FirstName);
        actual.First(c => c.Type.Equals(EmployerClaims.FamilyName)).Value.Should().Be(accountData.LastName);
        actual.First(c => c.Type.Equals(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier)).Value.Should().Be($"{accountData.FirstName} {accountData.LastName}");
        var accountClaims = actual.Where(c => c.Type.Equals(EmployerClaims.Account)).Select(c => c.Value).ToList();
        accountClaims.Count.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task Then_If_IsSuspended_Claim_Is_Marked_As_Suspended(
        string nameIdentifier,
        string emailAddress,
        EmployerUserAccounts accountData,
        [Frozen] Mock<IEmployerAccountService> accountService,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IOptions<EmployerProfilesWebConfiguration>> employerProfilesWebConfiguration,
        EmployerAccountPostAuthenticationClaimsHandler handler)
    {
        accountData.IsSuspended = true;
        var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, emailAddress);
        accountService.Setup(x => x.GetUserAccounts(nameIdentifier,emailAddress)).ReturnsAsync(accountData);
        
        var actual = await handler.GetClaims(tokenValidatedContext);
        
        accountService.Verify(x=>x.GetUserAccounts(nameIdentifier,emailAddress), Times.Once);
        actual.First(c => c.Type.Equals(ClaimTypes.AuthorizationDecision)).Value.Should().Be("Suspended");
    }


    private TokenValidatedContext ArrangeTokenValidatedContext(string nameIdentifier, string emailAddress)
    {
        var identity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
            new Claim(ClaimTypes.Email, emailAddress)
        });
        
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(identity));
        return new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
        {
            Principal = claimsPrincipal
        };
    }
    
    private class TestAuthHandler : IAuthenticationHandler
    {
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        public Task ChallengeAsync(AuthenticationProperties? properties)
        {
            throw new NotImplementedException();
        }

        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            throw new NotImplementedException();
        }
    }
}