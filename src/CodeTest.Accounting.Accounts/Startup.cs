using System;
using System.Net.Http;
using CodeTest.Accounting.Contracts;
using CodeTest.Accounting.Persistence;
using CodeTest.Accounting.ServiceClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Account = CodeTest.Accounting.Contracts.Account;

namespace CodeTest.Accounting.Accounts
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
            services.AddSingleton<IRepository<Account>, InMemoryRepository<Account>>();

            services.Configure<ServiceUrls>(Configuration.GetSection("ServiceUrls"));

            services.AddSingleton<CustomersServiceClient>((provider =>
            {
                var httpFactory = provider.GetService<IHttpClientFactory>();
                var urls = provider.GetService<IOptions<ServiceUrls>>();

                var client = httpFactory.CreateClient();
                client.BaseAddress = new Uri(urls?.Value.Customers ?? throw new ArgumentException("Missing Consumer Service URL setting."));

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

            services.AddControllers();
            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
