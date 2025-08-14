using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StarWarsTcg.Security
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(Configure => Configure.AddConsole()).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
/*
            //[StarWarsTcgApi.Api] configuration.GetConnectionString("IdentityConnection")
            string connectionString = "Server=10.0.0.209;Port=3306;database=SecureAuthDB;Uid=ralph;Pwd=ZR2!bHsFE";
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseMySQL(connectionString));

            services.AddIdentity<StarWarsTcg.Security.User, StarWarsTcg.Security.Role>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();
*/
        }

        public void Configure(IApplicationBuilder app)
        {
            //Log errors to the console
            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogError(ex, "An error occurred while processing the request.");
                    throw; // Re-throw the exception after logging it
                }
            });

            // Other middleware registrations
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}