using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using System.Xml;
using System.Web;
using umbraco.cms.businesslogic.packager.standardPackageActions;

namespace UMF.Code
{
    public class UmbracoStartup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Register custom MVC route for user profile
            //RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapRoute(
            //    "MemberProfile",                                                        // Route name
            //    "user/{profileURLtoCheck}",                                             // URL with parameters
            //    new { controller = "ProfileSurface", action = "RenderMemberProfile" }   // Parameter defaults
            //);
        }

        
    }
}