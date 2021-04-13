using KelimeOyunu.Business.Abstract;
using KelimeOyunu.Business.Concrete;
using KelimeOyunu.DataAccess.Abstract;
using KelimeOyunu.DataAccess.Concrete.ORMEntityFrameworkCore;
using KelimeOyunu.Entity;
using KelimeOyunu.WebUI.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI
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
            services.AddControllersWithViews();
            services.AddScoped<IGenericRepository<Kelime>, EFDal<Kelime, EFContext>>();
            services.AddScoped<IServiceBs<Kelime>, ManagerBs<Kelime>>();
            services.AddScoped<IGenericRepository<Oturum>, EFDal<Oturum, EFContext>>();
            services.AddScoped<IServiceBs<Oturum>, ManagerBs<Oturum>>();
            services.AddScoped<IGenericRepository<Surec>, EFDal<Surec, EFContext>>();
            services.AddScoped<IServiceBs<Surec>, ManagerBs<Surec>>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "KelimeOyunu.OID";
            });
            services.AddSignalR();
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
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RTHub>("/RTSorular");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
