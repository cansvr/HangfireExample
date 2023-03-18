using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;
using System;
using System.Configuration;

[assembly: OwinStartup(typeof(HangfireExample.Startup))]

namespace HangfireExample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            app.UseHangfireServer();

            var manager = new RecurringJobManager();
            manager.RemoveIfExists("HangfireJob");
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("HangfireJob!"), Cron.Daily);
            //RecurringJob.AddOrUpdate("HangfireJob", () => HangfireJob(), "*/0 3 * * *"); // "*/0 3 * * *" her gece 03:00'da çalışır.
            RecurringJob.AddOrUpdate("HangfireJob", () => HangfireJob(), Cron.Daily()); // Cron.Daily() her gece 00:00'da çalışır. Cron.Daily(3,0) olsaydı 03:00'da çalışırdı.
        }

        public void HangfireJob()
        {
            try
            {
                Console.WriteLine("test");
            }
            catch (Exception ex)
            {
            
            }
        }
    }

    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var owinContext = new OwinContext(context.GetOwinEnvironment());
            return true;
        }
    }
}
