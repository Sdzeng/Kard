﻿using Kard.Core.AppServices.Baiduspider;
using Kard.Core.Entities;
using Kard.DI;
using Kard.Extensions;
using Kard.Json;
using Kard.Runtime.Security.Authentication.WeChat;
using Kard.Web.Filters;
using Kard.Web.Middlewares.ImageHandle;
using Kard.Workers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.WebEncoders;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;

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


            //services.Configure<IISOptions>(options =>
            //{
            //    //options.ForwardClientCertificate = false;

            //});
            #region 跨域
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(new string[] { "*" }).
                        AllowAnyMethod().
                        AllowAnyHeader().

                        AllowCredentials();
                });
            });

            #endregion

            services.AddMvc(options =>
            {
                options.Filters.Add<GlobalAuthorizationFilter>();
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
                //全局异常过滤器
                options.Filters.Add<GlobalExceptionFilter>();
            });

            #region api文档
            if (env.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info
                    {
                        Version = "v1",
                        Title = "Kard接口文档",
                        Description = "RESTful API for Kard"
                    });

                    //Set the comments path for the swagger json and ui.
                    var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Kard.Web.xml");
                    c.IncludeXmlComments(filePath);

                });
            }

            //services.AddMvcCore().AddApiExplorer();
            #endregion



            #region 内存缓存
            services.AddMemoryCache();
            //services.AddImageHandle();
            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET 全局注册
          .AddSenparcWeixinServices(Configuration);//Senparc.Weixin 注册（如果使用Senparc.Weixin SDK则添加）
            #endregion

            #region 日志
            //services.AddLogging(builder =>
            //{
            //    builder.AddConfiguration(Configuration.GetSection("Logging"));
            //    //.AddFilter("Microsoft", LogLevel.Warning)
            //    //.AddDebug()
            //    //.AddConsole();
            //});

            #endregion

            #region Identity设置
            //services.Configure<IdentityOptions>(options =>
            //{
            //    // 密码配置
            //    options.Password.RequireDigit = true;//是否需要数字(0-9).
            //    options.Password.RequiredLength = 6;//设置密码长度最小为6
            //    options.Password.RequireNonAlphanumeric = true;//是否包含非字母或数字字符。
            //    options.Password.RequireUppercase = true;//是否需要大写字母(A-Z).
            //    options.Password.RequireLowercase = true;//是否需要小写字母(a-z).
            //    //options.Password.RequiredUniqueChars = 6;

            //    // 锁定设置
            //    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);//账户锁定时长30分钟
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);//账号锁定3天
            //    //options.Lockout.MaxFailedAccessAttempts = 3;//10次失败的尝试将账户锁定
            //    //options.Lockout.AllowedForNewUsers = true;

            //    // 用户设置
            //    options.User.RequireUniqueEmail = false; //是否Email地址必须唯一

            //});
            #endregion

            #region 未授权时使用的session cookie默认名称.AspNetCore.Session
            //services.AddSession(options =>
            //{
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


            //services.ConfigureApplicationCookie(options =>
            //{
            //    // Cookie settings
            //    options.Cookie.HttpOnly = false;
            //    options.Cookie.SameSite = SameSiteMode.None;
            //    //options.Cookie.Expiration = TimeSpan.FromMinutes(30);//30分钟
            //    //options.Cookie.Expiration = TimeSpan.FromDays(3);//3天
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            //    options.LoginPath = "/api/user/notlogin"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
            //    //options.LogoutPath = "/api/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
            //    //options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
            //    options.SlidingExpiration = true;
            //    //options.ExpireTimeSpan = TimeSpan.FromDays(3);
            //});
            #endregion

            #region 认证：你是谁？
            Action<CookieAuthenticationOptions> cookieSettingAction = (o) =>
            {
                o.Cookie.HttpOnly = true;//置为后台只读模式,前端无法通过JS来获取cookie值,可以有效的防止XXS攻击
                //o.Cookie.Domain = ".corntn.cn";
                o.LoginPath = "/login/notlogin";
                o.AccessDeniedPath = "/login/notlogin";
                o.SlidingExpiration = true;
                o.ExpireTimeSpan = TimeSpan.FromDays(7);  //当HttpContext.SignInAsync的IsPersistent = true 时生效
                //o.SessionStore = true;
            };
            //授权后使用的session cookie名称按选择的AuthenticationScheme（认证方案）定，比如.AspNetCore.Cookies 或  .AspNetCore.WeChatApp
            //AddCookie内部调用AddScheme
            //AddOAuth内部调用AddRemoteScheme（远程认证） AddGoogle AddFacebook AddTwitter内部都是调用AddOAuth
            services.AddAuthentication()
            //基于认证方案里的cookie认证
           .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieSettingAction);
            ////添加认证方案(scheme)2:wechatapp 
            //.AddCookie(WeChatAppDefaults.AuthenticationScheme, cookieSettingAction);


            //GDPR 《通用数据保护条例》（General Data Protection Regulation，简称GDPR）
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies 
            //    // is needed for a given request.
            //    options.CheckConsentNeeded = context => false;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            #endregion

            #region 授权：谁能做什么？Policy-->>每个Policy对应多个Requirement(条件)-->每个Requirement对应0或1个Handler
            //基于Scheme(认证方案)、角色的授权最终都会生成授权策略进行授权，授权策略本质上就是对Claims的一系列断言。
            // [Authorize(Roles = "admin")] 等同于下

            services.AddAuthorization(options =>
            {

                //同一个策略的上一个Handler（对应Requirement）明确表示验证失败后，是否继续执行下一个Handler,默认true
                options.InvokeHandlersAfterFailure = false;
                //策略授权
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));

                //每个策略下包含多个Requirement条件,每个Requirement对应0或1个Handler,
                //Handler里没有明确表示  context.Fail()或  context.Succeed(requirement),则授权失败；
                //Handler只要有一个context.Fail()则授权失败；
                //Handler里没有明确表示  context.Fail(),只要有context.Succeed(requirement)就成功；
                //Handler里没有
                //一个Authorize or Policy为一个策略
            });
            // [Authorize("AdminPolicy")] 

            //InvokeHandlersAfterFailure作用（一个Policy的handlers）
            //foreach (var handler in handlers)
            //{
            //    await handler.HandleAsync(authContext);
            //    if (!_options.InvokeHandlersAfterFailure && authContext.HasFailed)
            //    {
            //        break;
            //    }
            //}
            #endregion

            #region 静态资源压缩
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/plain", "text/html", "application/json" });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            #endregion

            #region 上传文件
            //services.Configure<FormOptions>(options =>
            //{
            //    options.ValueLengthLimit = int.MaxValue;
            //    options.MultipartBodyLengthLimit = long.MaxValue;
            //});
            #endregion

            #region IOC
            services.AddModule();

            services.TryAddScoped<IPasswordHasher<KuserEntity>, PasswordHasher<KuserEntity>>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            KardIoc.ServiceProvider = services.BuildServiceProvider();
            #endregion


            services.Configure<WebEncoderOptions>(options =>options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

        }

        // 请求管道会按顺序执行下列委托（中间件），返回顺序则相反；
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");
            var logger = LogManager.GetCurrentClassLogger(); 
            //var logger = loggerFactory.CreateLogger("Startup");
            logger.Info($"启动环境:{Serialize.ToJson(env)}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                #region api文档
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kard1.0接口文档");
                });
                #endregion
            }
            else
            {
                app.UseExceptionHandler("/error.html");
            }

            //app.UseImageHandle();
            //app.UseSession();
            //有反向代理时开启
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});

            app.UseCors("AllowSpecificOrigin");

            //GDPR规范
            //.UseCookiePolicy();


            app.UseAuthentication();


            // app.UseApiAuthorization();

            //DefaultFilesOptions options = new DefaultFilesOptions();
            //options.DefaultFileNames.Clear();
            //options.DefaultFileNames.Add("home.htm");
            //app.UseDefaultFiles(options);

            //var provider = new FileExtensionContentTypeProvider();
            //provider.Mappings.Add(".less", "text/css");

            IRegisterService register = RegisterService.Start(env, senparcSetting.Value).UseSenparcGlobal();// 启动 CO2NET 全局注册，必须！

            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);//微信全局注册，必须！




            app.UseStaticFiles(new StaticFileOptions
            {
                //添加图片处理
                FileProvider = new KardPhysicalFileProvider(env.WebRootPath, new ImageHandleOptions(), loggerFactory),
                // ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = $"max-age={ (60 * 60 * 24 * 7).ToString()}";
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
