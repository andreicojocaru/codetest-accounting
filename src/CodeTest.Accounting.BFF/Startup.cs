using System;
using System.Net.Http;
using CodeTest.Accounting.ServiceClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CodeTest.Accounting.BFF
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
            services.Configure<ServiceUrls>(Configuration.GetSection("ServiceUrls"));

            services.AddSingleton<AccountsServiceClient>((provider =>
            {
                var httpFactory = provider.GetService<IHttpClientFactory>();
                var urls = provider.GetService<IOptions<ServiceUrls>>();

                var client = httpFactory.CreateClient();
                client.BaseAddress = new Uri(urls?.Value.Accounts ?? throw new ArgumentException("Missing Accounts Service URL setting."));

                return new AccountsServiceClient(client);
            }));

            services.AddSingleton<CustomersServiceClient>((provider =>
            {
                var httpFactory = provider.GetService<IHttpClientFactory>();
                var urls = provider.GetService<IOptions<ServiceUrls>>();

                var client = httpFactory.CreateClient();
                client.BaseAddress = new Uri(urls?.Value.Customers ?? throw new ArgumentException("Missing Consumers Service URL setting."));

                return new CustomersServiceClient(client);
            }));


            services.AddSingleton<TransactionsServiceClient>((provider =>
            {
                var httpFactory = provider.GetService<IHttpClientFactory>();
                var urls = provider.GetService<IOptions<ServiceUrls>>();

                var client = httpFactory.CreateClient();
                client.BaseAddress = new Uri(urls?.Value.Transactions ?? throw new ArgumentException("Missing Transactions Service URL setting."));

                return new TransactionsServiceClient(client);
            }));

            services.AddHttpClient();
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
