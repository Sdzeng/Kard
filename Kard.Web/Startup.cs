using Kard.Core.Entities;
using Kard.Extensions;
using Kard.Web.Middlewares.ImageHandle;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NLog.Extensions.Logging;
using System;
using System.IO.Compression;
using System.Linq;

//dotnet publish --framework netcoreapp2.0 --output "D:\GitRepository\Kard.Publish" --configuration Release
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


            services.Configure<IISOptions>(options =>
            {
                //options.ForwardClientCertificate = false;

            });


            services.AddMvc();


         

            services.AddMemoryCache();
            //services.AddImageHandle();
            services.AddLogging(builder =>
            {
                builder
                    .AddConfiguration(Configuration.GetSection("Logging"));
                    //.AddFilter("Microsoft", LogLevel.Warning)
                    //.AddDebug()
                    //.AddConsole();
            });
           
            //services.AddSession(options =>
            //{
            //    options.Cookie.Name = ".Kard.Session";
            //    options.IdleTimeout = TimeSpan.FromDays(7);
            //});
            //if (env.IsProduction())
            //{
            //    services.AddApiAuthorization(options =>
            //    {
            //        options.PathMatch = "/user";
            //    });
            //}
            //else
            //{
            //    services.AddApiAuthorization(options =>
            //    {
            //        var alipaySectionConfig = Configuration.GetSection("ApiAuthorization");
            //        options.PathMatch = alipaySectionConfig["PathMatch"];
            //    });
            //}

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
           {
             o.LoginPath = "/user/login";
             o.AccessDeniedPath = "/user/login";
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
            services.AddSingleton<IPasswordHasher<KuserEntity>, PasswordHasher<KuserEntity>>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // 请求管道会按顺序执行下列委托（中间件），返回顺序则相反；
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //添加NLog
            loggerFactory.AddNLog();
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            //var logger=loggerFactory.CreateLogger<Startup>();
            //logger.LogInformation("开始启动");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
               app.UseExceptionHandler("/error.htm");
            }

            //app.UseImageHandle();
            //app.UseSession();
            app.UseAuthentication();
            // app.UseApiAuthorization();
            
             DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("home.htm");
            app.UseDefaultFiles(options);

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".less", "text/css");
         
 
            app.UseStaticFiles(new StaticFileOptions
            {
                //添加图片处理
                FileProvider = new KardPhysicalFileProvider(env, new ImageHandleOptions(), loggerFactory),
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
