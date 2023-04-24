using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Employer.Profiles.Domain.Configuration;
using SFA.DAS.Employer.Profiles.Web.AppStart;
using SFA.DAS.Employer.Profiles.Web.Extensions;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.GovUK.Auth.AppStart;

var builder = WebApplication.CreateBuilder(args);

var rootConfiguration = builder.Configuration.LoadConfiguration();

builder.Services.AddOptions();
builder.Services.Configure<EmployerProfilesWebConfiguration>(rootConfiguration.GetSection(nameof(EmployerProfilesWebConfiguration)));
builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerProfilesWebConfiguration>>().Value);

builder.Services.AddServiceRegistration();
builder.Services.AddAuthenticationServices();

builder.Services.AddLogging();
builder.Services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

builder.Services.AddAndConfigureGovUkAuthentication(
    rootConfiguration, 
    typeof(EmployerAccountPostAuthenticationClaimsHandler),
    "",
    "/service/account-details");
            
builder.Services.AddMaMenuConfiguration(RouteNames.SignOut, 
    rootConfiguration["ResourceEnvironmentName"]);
            
builder.Services.Configure<RouteOptions>(options =>
{
                
}).AddMvc(options =>
{
    //options.Filters.Add(new GoogleAnalyticsFilter());
    if (!rootConfiguration.IsDev())
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());    
    }
                    
});


if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection(rootConfiguration);
    
}
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();
            
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
}

app.UseAuthentication();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpointBuilder =>
{
    endpointBuilder.MapControllerRoute(
        name: "default",
        pattern: "{controller=Service}/{action=Index}/{id?}");
});

app.Run();