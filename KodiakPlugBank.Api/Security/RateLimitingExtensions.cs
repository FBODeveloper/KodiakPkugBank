using System.Net;
using System.Threading.RateLimiting;

namespace KodiakPlugBank.Api.Security;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddSecurityRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection(RateLimitingSettings.SectionName).Get<RateLimitingSettings>()
            ?? new RateLimitingSettings();
        services.AddSingleton(settings);

        services.AddRateLimiter(limiter =>
        {
            limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            limiter.OnRejected = async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        retryAfter.TotalSeconds.ToString("0");
                }
                else
                {
                    var settings = context.HttpContext.RequestServices
                        .GetRequiredService<RateLimitingSettings>();
                    var window = IsBootstrap(context.HttpContext)
                        ? settings.Bootstrap.WindowSeconds
                        : settings.Global.WindowSeconds;
                    context.HttpContext.Response.Headers.RetryAfter = window.ToString();
                }

                var logger = context.HttpContext.RequestServices
                    .GetService<ILoggerFactory>()?.CreateLogger("RateLimiter");
                logger?.LogWarning(
                    "Rate limit atingido (429) para {Method} {Path} do IP {RemoteIp}",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    context.HttpContext.Connection.RemoteIpAddress);

                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new { message = "Muitas requisições em pouco tempo. Tente novamente mais tarde." },
                    token);
            };

            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    context.Connection.RemoteIpAddress ?? IPAddress.IPv6Loopback,
                    _ => BuildOptions(settings.Global)));

            limiter.AddPolicy(PolicyNames.Bootstrap, BuildPartitionResolver(settings.Bootstrap));
        });

        return services;
    }

    private static bool IsBootstrap(HttpContext context) =>
        context.Request.Method == HttpMethods.Post &&
        context.Request.Path.Equals("/api/v1/payer", StringComparison.OrdinalIgnoreCase);

    private static Func<HttpContext, RateLimitPartition<IPAddress>> BuildPartitionResolver(RateLimitingOptions options)
    {
        return context => RateLimitPartition.GetSlidingWindowLimiter(
            context.Connection.RemoteIpAddress ?? IPAddress.IPv6Loopback,
            _ => BuildOptions(options));
    }

    private static SlidingWindowRateLimiterOptions BuildOptions(RateLimitingOptions options)
    {
        return new SlidingWindowRateLimiterOptions
        {
            PermitLimit = options.PermitLimit,
            Window = TimeSpan.FromSeconds(options.WindowSeconds),
            SegmentsPerWindow = Math.Max(1, options.SegmentsPerWindow),
            QueueLimit = options.QueueLimit,
            AutoReplenishment = true
        };
    }
}
