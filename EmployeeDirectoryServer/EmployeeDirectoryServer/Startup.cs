using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmployeeDirectoryServer {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            // app.UseHttpsRedirection(); // http

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.Run(async (context) => {
                await context.Response.WriteAsync("Hello World!");
            });
           
            // пустой проект
            // app.UseRouting();
            /*
            app.UseEndpoints(endpoints => {
                endpoints.MapGet("/", async context => {
                    await context.Response.WriteAsync("Hello World!");
                });
            });*/
        }
    }
}
