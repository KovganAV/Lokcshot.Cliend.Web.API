using Lokcshot.Cliend.Web.API.Core.Interfaces;
using Lokcshot.Cliend.Web.API.Settings;
using Refit;

namespace discord.client.webapi
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            var microservicesSettings = new MicroservicesSettings();
            configuration.GetSection("MicroservicesSettings").Bind(microservicesSettings);

            services.AddRefitClient<IUserRefit>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(microservicesSettings.UserApiUrl));

            services.AddRefitClient<IBlobStorageRefit>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(microservicesSettings.BlobStorageUrl));

            services.AddRefitClient<IServersRefit>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(microservicesSettings.ServersApiUrl));

            services.AddRefitClient<IBotsRefit>()
                .ConfigureHttpClient(c => {
                    c.BaseAddress = new Uri(microservicesSettings.BotsApiUrl);
               });

            return services;
        }
    }
}
