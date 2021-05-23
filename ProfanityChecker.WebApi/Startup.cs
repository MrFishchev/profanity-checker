using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProfanityChecker.Infrastructure;
using ProfanityChecker.Logic;
using Serilog;

namespace ProfanityChecker.WebApi
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ProfanityChecker.WebApi", Version = "v1"});
            });

            services.AddDbContext<DataContext>(
                options => options.UseSqlite(@"Data Source=db/profanity_checker.db")
            );
            
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IBannedPhraseRepository, BannedPhraseRepository>();
            
            services.AddTransient<IFileService, FileService>();
            services.AddScoped<IProfanityService, ProfanityService>();
            services.AddSingleton<IAlgorithmFactory, AlgorithmFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProfanityChecker.WebApi v1"));

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}