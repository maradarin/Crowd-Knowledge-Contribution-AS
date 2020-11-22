using System.Web;
using System.Web.Mvc;

namespace Crowd_Knowledge_Contribution_AS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
