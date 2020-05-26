using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mvc2Integrations.Models;
using Mvc2Integrations.Services;
using Mvc2Integrations.Services.Currency;
using Mvc2Integrations.Services.KrisInfo;

namespace Mvc2Integrations
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
            services.AddDbContext<HockeyDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddMemoryCache();
            services.AddTransient<IInfoService, InfoService>();
            services.Decorate<IInfoService, CachedInfoService>();

            services.Configure<Settings>(Configuration.GetSection("CurrencySettings"));
            services.Configure<KrisInfoSettings>(Configuration.GetSection("KrisInfoSettings"));

            services.AddTransient<ICurrencyCalculator, CurrencyCalculatorFake>();
            //services.Decorate<ICurrencyCalculator, RetryCurrencyCalculator>();
            //services.Decorate<ICurrencyCalculator, CachedCurrencyCalculator>();
            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
