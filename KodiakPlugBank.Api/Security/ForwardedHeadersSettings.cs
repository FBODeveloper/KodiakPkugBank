using System.Net;
using System.Threading.RateLimiting;

namespace KodiakPlugBank.Api.Security;

public sealed class ForwardedHeadersSettings
{
    public const string SectionName = "ForwardedHeaders";

    public int ForwardLimit { get; set; } = 1;
    public List<string> KnownProxies { get; set; } = [];
    public List<string> KnownNetworks { get; set; } = [];
}
