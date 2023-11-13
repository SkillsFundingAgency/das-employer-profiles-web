using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Employer.Profiles.Web.Extensions;

namespace SFA.DAS.Employer.Profiles.Web.Filters;

public class AnalyticsFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller as Controller;
        if (controller != null)
        {
            var user = controller.User;
            var userId = user?.GetUserId();
            controller.ViewBag.GaData = new GaData
            {
                UserId = userId,
            };
        }

        base.OnActionExecuting(context);
    }

    public class GaData
    {
        public string DataLoaded { get; set; } = "dataLoaded";
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string Vpv { get; set; }
        public string Acc { get; set; }

        public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
    }
}