using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MMAEvents.ApiClients;
using MMAEvents.TelegramBot.Commands;
using MMAEvents.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MMAEvents.TelegramBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TelegramBotClient>(
                sp => new TelegramBotClient(Configuration.GetSection("TelegramBot_Key").Value));
            services.AddTransient<TelegramBotService>();

            services.AddHttpClient();
            services.AddSingleton<EventsApiClient>(
                sp => new EventsApiClient(
                    Configuration.GetSection("EventsApi:BaseUrl").Value, sp.GetRequiredService<HttpClient>()));
            services.AddSingleton<IEventsApiClient, CachedApiService>();

            services.AddSingleton<CommandsCollection>();

            services.AddMemoryCache();
            services.AddControllers().AddNewtonsoftJson();

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<EventChangesService>();
            });
        }        
    }
}
