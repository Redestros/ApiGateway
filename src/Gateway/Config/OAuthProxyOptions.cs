namespace Gateway.Config;

public class OAuthProxyOptions
{
    public const string SectionName = "OAuthProxy";

    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string NameClaim { get; set; } = "preferred_username";
    public string RoleClaim { get; set; } = "roles";
}