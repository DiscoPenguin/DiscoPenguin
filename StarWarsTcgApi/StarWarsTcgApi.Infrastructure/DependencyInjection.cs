using System.Security.Policy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using StarWarsTcg.Security;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Infrastructure.Data;
using StarWarsTcgApi.Infrastructure.Repositories;
using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddLogging(Configure => Configure.AddConsole()).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
            try
            {
                services.AddSingleton(new MySqlDataAccess(configuration));

/*
                // Add Identity Services
                services.AddIdentity<StarWarsTcg.Security.User, StarWarsTcg.Security.Role>()
                    .AddEntityFrameworkStores<StarWarsTcg.Security.IdentityDbContext>()
                    .AddDefaultTokenProviders();
*/
                // Register Repositories
                services.AddScoped<ICardRepository, CardRepository>();
                services.AddScoped<IDeckRepository, DeckRepository>();
                
                services.AddScoped<IDeckCardRepository, DeckCardRepository>();
                services.AddScoped<IDeckBuilderRepository, DeckBuilderRepository>();

                services.AddScoped<IGameRepository, GameRepository>();
                services.AddScoped<IGameCardRepository, GameCardRepository>();
                services.AddScoped<IGameLogRepository, GameLogRepository>();

                services.AddScoped<IAssetRepository, AssetRepository>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                services.AddScoped<IPlayerRepository, PlayerRepository>();
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it
                Console.WriteLine("An error occurred while configuring the StarWarsTcgApi.Infrastructure services.", ex);
                throw;
            }

            return services;
        }
    }
}