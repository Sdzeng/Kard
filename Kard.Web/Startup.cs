using Kard.Extensions;
using Kard.Web.Middlewares.ApiAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.IO.Compression;
using System.Linq;

namespace Kard.Web
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
            IHostingEnvironment env = services.GetHostingEnvironment();
            //ASP.NET 提供的功能和中间件，例如 MVC，遵循约定——使用一个单一的 AddService扩展方法来注册所有该功能所需的服务。
            //IMvcBuilder mvcBuilder= services.AddMvc();
            //string assemblyParts = string.Join(",", mvcBuilder.PartManager.ApplicationParts.Select(a=>a.Name));
         
            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".Kard.Session";
                options.IdleTimeout = TimeSpan.FromDays(7);
            });
            services.AddApiAuthorization(options => {
                options.PathMatch = "/user";
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/plain", "text/html", "application/json" });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.AddModule();
        }

        // 请求管道会按顺序执行下列委托（中间件），返回顺序则相反；
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
               app.UseExceptionHandler("/error.htm");
            }

            app.UseApiAuthorization();

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.htm");
            app.UseDefaultFiles(options);

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".less", "text/css");
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =$"max-age={ (60 * 60 * 24 * 7).ToString()}";
                }
            });

            app.UseResponseCompression();
            app.UseMvc();

        
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseBrowserLink();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}

            //app.UseStaticFiles();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
