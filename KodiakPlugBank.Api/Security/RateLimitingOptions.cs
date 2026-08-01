namespace KodiakPlugBank.Api.Security;

public sealed class RateLimitingSettings
{
    public const string SectionName = "RateLimiting";

    public RateLimitingOptions Global { get; set; } = new();
    public RateLimitingOptions Bootstrap { get; set; } = new();
}

public sealed class RateLimitingOptions
{
    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 10;
    public int SegmentsPerWindow { get; set; } = 1;
    public int QueueLimit { get; set; } = 0;
}

public static class PolicyNames
{
    public const string Bootstrap = "bootstrap";
}
