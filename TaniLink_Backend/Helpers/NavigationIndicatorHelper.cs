using Microsoft.AspNetCore.Mvc;

namespace TaniLink_Backend.Helpers
{
    public static class NavigationIndicatorHelper
    {
        public static string MakeActiveClass(this IUrlHelper urlHelper, string controller, string action, string area = null)
        {
            try
            {
                string result = "active";

                string routeArea = urlHelper.ActionContext.RouteData.Values["area"]?.ToString();
                string routeController = urlHelper.ActionContext.RouteData.Values["controller"]?.ToString();
                string routeAction = urlHelper.ActionContext.RouteData.Values["action"]?.ToString();

                if (string.IsNullOrEmpty(routeController) || string.IsNullOrEmpty(routeAction))
                    return null;

                // Check area only if it's provided
                if ((string.IsNullOrEmpty(area) || string.Equals(routeArea, area, StringComparison.OrdinalIgnoreCase)) &&
                    string.Equals(routeController, controller, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(routeAction, action, StringComparison.OrdinalIgnoreCase))
                {
                    return result;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
