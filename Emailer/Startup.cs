using Emailer.Helpers;
using Emailer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;

namespace Emailer
{
    public class Startup
    {
        readonly string CorsAllowedOriginsPolicy = "_corsAllowedOriginsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup - CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsAllowedOriginsPolicy,
                    builder =>
                    {
                        // TODO: limit CORS
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            // Setup - Configuration Settings
            var _emailsettings = new EmailSettings();
            Configuration.GetSection("Email").Bind(_emailsettings);
            services.AddSingleton(_emailsettings);

            // Setup - DI Services
            services.AddScoped<ITokenizationService, TokenizationService>();
            services.AddScoped(implementationFactory =>
            {
                return new SendGridClient(Configuration.GetValue<string>("Email:SENDGRID_API_KEY"));
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(CorsAllowedOriginsPolicy);
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
