using System.Web.Mvc;

namespace GF.UCenter.Web.Api
{
    /// <summary>
    /// Filter configuration
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Register global filters
        /// </summary>
        /// <param name="filters">The filter collection.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}