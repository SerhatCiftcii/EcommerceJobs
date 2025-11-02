using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace ECommerceSolution.Api.Authorization
{
    //geliştirme ortamı için    
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
    //alt tarafını şimdlik bıraktım herkez için açık yaptım geliştirme ortamı.
    //    {
    //    // Hangfire Dashboard'a erişimi kontrol eden filtre
    //    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    //    {
    //        public bool Authorize(DashboardContext context)
    //        {
    //            var httpContext = context.GetHttpContext();

    //            // Kullanıcı kimliği doğrulanmış mı kontrol et
    //            if (httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated)
    //            {
    //                // Kullanıcı "Admin" rolüne sahip mi kontrol et
    //                return httpContext.User.IsInRole("Admin");
    //            }

    //            // Giriş yapılmamışsa erişim reddedilir
    //            return false;
    //        }
    //    }

   
    public static class DashboardContextExtensions
    {
        public static HttpContext GetHttpContext(this DashboardContext context)
        {
            var httpContextObj = context.GetType().GetProperty("HttpContext")?.GetValue(context, null);
            return httpContextObj as HttpContext;
        }
    }

}
