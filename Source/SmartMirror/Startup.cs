using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartMirror.Data;
using SmartMirror.Data.Calendar;
using SmartMirror.Data.Fitbit;
using SmartMirror.Data.Fuel;
using SmartMirror.Data.Jokes;
using SmartMirror.Data.News;
using SmartMirror.Data.Soccer;
using SmartMirror.Data.Spotify;
using SmartMirror.Data.StockData;
using SmartMirror.Data.VVS;
using SmartMirror.Data.WeatherForecast;
using SmartMirror.SmartHome.Hue;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace SmartMirror
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(p => p.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

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

            services.AddSingleton<HttpClient>();

            var redisSection = Configuration.GetSection(nameof(RedisConfiguration));
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

            services.Configure<WeatherConfiguration>(Configuration.GetSection(nameof(WeatherConfiguration)));
            services.Configure<FuelConfiguration>(Configuration.GetSection(nameof(FuelConfiguration)));
            services.Configure<SpotifyConfiguration>(Configuration.GetSection(nameof(SpotifyConfiguration)));
            services.Configure<FitbitConfiguration>(Configuration.GetSection(nameof(FitbitConfiguration)));
            services.Configure<CalendarConfiguration>(Configuration.GetSection(nameof(CalendarConfiguration)));
            services.Configure<NewsConfiguration>(Configuration.GetSection(nameof(NewsConfiguration)));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
            })
            .AddDefaultTokenProviders()
            .AddUserManager<InMemoryUserManager>()
            .AddUserStore<InMemoryUserStore>()
            .AddRoleStore<InMemoryRoleStore>();

            //services.AddAuthentication()
            //    .AddCookie("cookie")
            //    .AddOpenIdConnect("", p =>
            //    {
            //    });

            //services.AddAuthorization(p =>
            //    p.AddPolicy("spotify", p => p
            //        .AddAuthenticationSchemes("spotify_scheme")
            //        .RequireAuthenticatedUser()
            //    )
            //);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                new CultureInfo("de-DE")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("de-DE"),
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
                endpoints.MapBlazorHub(); //.RequireAuthorization("spotify");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
