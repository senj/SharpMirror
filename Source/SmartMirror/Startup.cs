using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartMirror.Data;
using SmartMirror.Data.Bring;
using SmartMirror.Data.Calendar;
using SmartMirror.Data.Fitbit;
using SmartMirror.Data.Fuel;
using SmartMirror.Data.GoogleFit;
using SmartMirror.Data.Jokes;
using SmartMirror.Data.News;
using SmartMirror.Data.Routes;
using SmartMirror.Data.Soccer;
using SmartMirror.Data.Speech;
using SmartMirror.Data.Spotify;
using SmartMirror.Data.StockData;
using SmartMirror.Data.VVS;
using SmartMirror.Data.WeatherForecast;
using SmartMirror.FakeAspNetIdentity;
using SmartMirror.SmartHome.Hue;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace SmartMirror
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(p => p.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddLocalization(p => p.ResourcesPath = "Resources");

            services.AddSingleton<IntentExecutor>();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<VvsService>();
            services.AddSingleton<FuelService>();
            services.AddSingleton<CalendarService>();
            services.AddSingleton<NewsService>();
            services.AddSingleton<JokesQuotesService>();
            services.AddSingleton<BundesligaService>();
            services.AddSingleton<StockDataService>();
            services.AddSingleton<HueService>();
            services.AddSingleton<FitbitService>();
            services.AddSingleton<BringService>();
            services.AddSingleton<RouteService>();
            services.AddSingleton<SpeechRecognitionService>();
            services.AddSingleton<GoogleFitService>();
            services.AddSingleton<SpotifyService>();

            services.AddSingleton<HttpClient>();

            var redisSection = _configuration.GetSection(nameof(RedisConfiguration));
            RedisConfiguration redisConfiguration = new RedisConfiguration();
            redisSection.Bind(redisConfiguration);

            if (redisConfiguration.DisableCache)
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfiguration.Configuration;
                    options.InstanceName = redisConfiguration.InstanceName;
                });
            }

            services.Configure<WeatherConfiguration>(_configuration.GetSection(nameof(WeatherConfiguration)));
            services.Configure<FuelConfiguration>(_configuration.GetSection(nameof(FuelConfiguration)));
            services.Configure<SpotifyConfiguration>(_configuration.GetSection(nameof(SpotifyConfiguration)));
            services.Configure<FitbitConfiguration>(_configuration.GetSection(nameof(FitbitConfiguration)));
            services.Configure<CalendarConfiguration>(_configuration.GetSection(nameof(CalendarConfiguration)));
            services.Configure<NewsConfiguration>(_configuration.GetSection(nameof(NewsConfiguration)));
            services.Configure<ProfileConfiguration>(_configuration.GetSection(nameof(ProfileConfiguration)));
            services.Configure<BringConfiguration>(_configuration.GetSection(nameof(BringConfiguration)));
            services.Configure<RouteConfiguration>(_configuration.GetSection(nameof(RouteConfiguration)));
            services.Configure<SpeechRecognitionConfiguration>(_configuration.GetSection(nameof(SpeechRecognitionConfiguration)));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
            })
            .AddDefaultTokenProviders()
            .AddUserManager<InMemoryUserManager>()
            .AddUserStore<InMemoryUserStore>()
            .AddRoleStore<InMemoryRoleStore>()
            .AddSignInManager<InMemorySignInManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("de"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                // NOTE: Set culture here. Your browser must also be in this culture.
                // You can also set a cookie: .AspNetCore.Culture | c=en-US|uic=en-US
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
