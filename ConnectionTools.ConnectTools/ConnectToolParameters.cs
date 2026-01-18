namespace ConnectionTools.ConnectTools;

public sealed class ConnectToolParameters
{
    private ConnectToolParameters(string siteRootAddress, string userName, string password)
    {
        SiteRootAddress = siteRootAddress;
        UserName = userName;
        Password = password;
    }

    public string SiteRootAddress { get; }
    public string UserName { get; }
    public string Password { get; }

    public static ConnectToolParameters? Create(string? siteRootAddress, string? userName, string? password)
    {
        if (string.IsNullOrWhiteSpace(siteRootAddress) || string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
            return null;
        return new ConnectToolParameters(siteRootAddress, userName, password);
    }
}