using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Application.Services;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using StarWarsTcgApi.Infrastructure.Repositories;
using StarWarsTcgApi.Infrastructure.Services; // Need this for ICardRepository in DeckService ValidateDeck method

namespace StarWarsTcgApi.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddLogging(Configure => Configure.AddConsole()).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
            try
            {
                // Register Application Services
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IPlayerService, PlayerService>();
                services.AddScoped<IGameService, GameService>();
                services.AddScoped<IDeckService, DeckCardService>();
                services.AddScoped<ICardService, CardService>();
                services.AddScoped<IAssetService, AssetService>();

                services.AddScoped<IDeckBuilderService, DeckBuilderService>();

                // As mentioned before, for validation logic that spans multiple services,
                // consider creating a dedicated service here:
                // services.AddScoped<IDeckValidationService, DeckValidationService>();
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it
                Console.WriteLine("An error occurred while configuring the StarWarsTcgApi.Application services.", ex);
                throw;
            }
            return services;
        }
    }
}