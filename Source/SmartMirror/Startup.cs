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
using SmartMirror.Data.Clock;
using SmartMirror.Data.Fitbit;
using SmartMirror.Data.Fuel;
using SmartMirror.Data.GoogleFit;
using SmartMirror.Data.Jokes;
using SmartMirror.Data.Layout;
using SmartMirror.Data.News;
using SmartMirror.Data.Routes;
using SmartMirror.Data.Soccer;
using SmartMirror.Data.Speech;
using SmartMirror.Data.Spotify;
using SmartMirror.Data.StockData;
using SmartMirror.Data.TempSensor;
using SmartMirror.Data.VVS;
using SmartMirror.Data.WeatherForecast;
using SmartMirror.FakeAspNetIdentity;
using SmartMirror.SmartHome.Hue;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace SmartMirror
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;

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

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly);
            });

            // Services
            services.AddSingleton<IntentExecutor>();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<VvsService>();
            services.AddSingleton<FuelService>();
            services.AddSingleton<CalendarService>();
            services.AddSingleton<NewsService>();
            services.AddSingleton<JokesQuotesService>();
            services.AddSingleton<SoccerService>();
            services.AddSingleton<StockDataService>();
            services.AddSingleton<HueService>();
            services.AddSingleton<FitbitService>();
            services.AddSingleton<BringService>();
            services.AddSingleton<RouteService>();
            services.AddSingleton<SpeechRecognitionService>();
            services.AddSingleton<GoogleFitService>();

            // State
            services.AddSingleton<BringState>();
            services.AddSingleton<WeatherState>();
            services.AddSingleton<RouteState>();
            services.AddSingleton<HueState>();
            services.AddSingleton<CalendarState>();
            services.AddSingleton<FuelState>();
            services.AddSingleton<ClockState>();
            services.AddSingleton<SpotifyState>();
            services.AddSingleton<VvsState>();
            //services.AddSingleton<FitbitState>();
            services.AddSingleton<NewsState>();
            services.AddSingleton<GoogleFitState>();
            services.AddSingleton<SoccerState>();
            services.AddSingleton<LayoutState>();
            services.AddSingleton<TempState>();

            _httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            services.AddSingleton(_httpClient);

            IConfigurationSection redisSection = _configuration.GetSection(nameof(RedisConfiguration));
            RedisConfiguration redisConfiguration = new();
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

            services.Configure<GoogleApiConfiguration>(_configuration.GetSection(nameof(GoogleApiConfiguration)));
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopped.Register(OnApplicationStopped);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            CultureInfo[] supportedCultures = new[]
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

        private void OnApplicationStopped()
        {
            _httpClient?.Dispose();
            _httpClient = null;
        }
    }
}
