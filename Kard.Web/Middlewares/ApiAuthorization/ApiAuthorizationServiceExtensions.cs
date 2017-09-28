using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kard.Web.Middlewares.ApiAuthorization
{
    /// <summary>
    /// 添加到DI
    /// </summary>
    public static class ApiAuthorizationServiceExtensions
    {

        //public static IServiceCollection AddApiAuthorized(this IServiceCollection services)
        //{
        //    if (services == null)
        //    {
        //        throw new ArgumentNullException(nameof(services));
        //    }

        //  return  services.AddApiAuthorized(options =>
        //    {
        //        options.ApiPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoyk+FKeZz3ZPdTAUVgh6yuDA9GwIUhzW7tPHDC6+LVl1eE3ZTlhSVRdSzwSsPSHHUg3Vmit+oMN8rynqUO+N+9fysdkvyLxw1a3duYHkR5bQpPa/qkyf1iug2LZsNPUJqSvy4nP9mziIepZvB/K5KK0pxKH3tjZywwN0DKgzdQRDoN/azGKLXQfzOM7PjzXACvrb4Jm6aGaOOEOUnyw9gS7LVBcMJiSKW6Roi6HvXiBLuEjYwD6bAUI/QVPOHhgE3ALR4UOjgnMVZK/E5yCe5B+81TBwt55qbHMRn3hJXrD8W9nNz8gtZHjvIMJswgWRkw7mFvq/2UgFimPLg7+z2wIDAQAB";
        //        options.MerchantPrivateKey = "MIIEowIBAAKCAQEAoNuNaJ6EyuvH/5DJ9czVhD7k4CCy5M982ob48qJ5UJ4i+D6w4jSEEICtgWpShX7yPIDMtrC37C9Im6iQKNptbAcSkkbIWBkd+FT0TSvgRYq1Y5EW0GPG/fTqpDvkVZ2kgrHn+0pclbmhuzTEFTXEPP/aJ96N6kASllewcueXxRh1u4SEZa1baW3hj2b9ILp/dUOmYVqYGI6GoyZxFS77tybD4IliOjNNmpwcn20H9m88jfWGqAax7Pyhqye8HxmYQjid/O/aBYmsyBc6KtOYLTPtWClrl7/R6hqxFuyWIMubEennOrYPLVduLz5hYf466MpwdX0lOWVTOcijh+sNawIDAQABAoIBAHTkt5FESMnRp8gPZsRJswvkTJyXBomCQn/nP7fK3A/6qvztSctIUIRRjkSSiB9czhmrm5hTOYTYI5KCXyhg+s1bImLML0DeF8pjqhj+fn7qur/9KrALIgtg1La+k0KoSWl0cVlKg5eGi4IoaSnkpZOSde0tLMIDB77bQG1v4PmnkshHhAeZyyFv1eT03T4gR76gE5BWi8aKK1w+p/yeTahQgTaetMaUkvZcjF18HxNIr3TggdgCHVgKPVGzK4DJrIaz7YYRbel/WKBhYwKTcUUxPkL0hSTskD+Du4N+QrwsKrbZCZYsk4EmfMakysExIhSgiFeWI6zjN26nGvAAevkCgYEAzcI7WcBIX+XnoQqMkxmB3O5YxR+hueu8FxENCxtqcmH+VTtCRK2j99++j4H/WaPkI7bbbqQ6l+r+B0tFjLOD3q5Lb6Apl2GeonZqg9Hxjnh8/x0BNokj6Yi3CI9EmUzzDl5/FsAJc0JpGTrjxjWwiQpt8hsqEi86uimVCM/+H0UCgYEAyCKZVoN0oan2rNwct8AIb1wanf+WwWPGN96MCsa/uVueQw5w7vAkL4A+pxJ+DnrAiNkqj1gnC3z6mhLn/U/JwENBqfZ0kPFvMSxizKeuznYE9Nd3p3EQqhBXFzTqPL+j/CGeQQFTSfjoKU+/YXKPRFeamatrZBVZAFs2HencLO8CgYABhDPxxmQCKOybGDvO/boiSbNnyILlnuIk/WAuO8Z+D1DTiftEDE+QDRsXbarXG0kcJkXZu+YepoG2xgw+LDiFlJ1Dtld4ISUNJ3hDfnGcTjEFpE4U+8/C8dn/eybTRmjoKzQek82+BrhFklSJam3hpK0IwsB9n9F7+8B4byf9TQKBgEfMGcRunm3MwqDayjalXqFMooPiCFxShe7PGOBY0rkVhB48VQly/xhHYrKfMLfTE7VEaz9HeaJmcKTLeYxnjlI6DpmBv+mro585dCxFT7Hjpv2LOvLH3hmUiRnbMtkeSx8NlG9voUidUfRYFqlH5tu2rdDJLVqXSfmVv0FV/1XjAoGBAI5DjrJRdLlSn1gSqanGHBvORmkiMk1coH88H6fStAgqHhqnUsiYlDGYUlI0GesoyzSQmt/GHvQhCU6CF9Kx1NpCf5zvMvant47Jxa8QTA1gRvCJVTjjfnVDqqve8pfluM/uLsJvqkqWCGmyok2GBCndMj3vsBEorM//e7gs9qzI";
        //        options.MerchantPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoNuNaJ6EyuvH/5DJ9czVhD7k4CCy5M982ob48qJ5UJ4i+D6w4jSEEICtgWpShX7yPIDMtrC37C9Im6iQKNptbAcSkkbIWBkd+FT0TSvgRYq1Y5EW0GPG/fTqpDvkVZ2kgrHn+0pclbmhuzTEFTXEPP/aJ96N6kASllewcueXxRh1u4SEZa1baW3hj2b9ILp/dUOmYVqYGI6GoyZxFS77tybD4IliOjNNmpwcn20H9m88jfWGqAax7Pyhqye8HxmYQjid/O/aBYmsyBc6KtOYLTPtWClrl7/R6hqxFuyWIMubEennOrYPLVduLz5hYf466MpwdX0lOWVTOcijh+sNawIDAQAB";
        //        options.AppId = "2017072707918310";
        //        options.UserOauthUri = "https://openauth.alipay.com/oauth2/publicAppAuthorize.htm";
        //        options.AppRedirectUri = "http://tapi.wincustomer.net/alipay/auth";
        //        options.ApiServerUrl = "https://openapi.alipay.com/gateway.do";
        //        options.SignType = "RSA2";
        //    });
        //}

        public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services;
        }


        public static IServiceCollection AddApiAuthorization(this IServiceCollection services, Action<ApiAuthorizationOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new ApiAuthorizationOptions();
            configureOptions(options);


            if (options.PathMatch.HasValue && options.PathMatch.Value.EndsWith("/", StringComparison.Ordinal))
            {
                throw new ArgumentException("The path must not end with a '/'", nameof(options.PathMatch));
            }


            services.Configure(configureOptions);

            return services;
        }



    }
}
