namespace ConnectionTools.ConnectTools.Tests;

public sealed class ConnectToolParametersTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsInstance()
    {
        // Arrange
        const string siteRootAddress = "https://example.com";
        const string userName = "testuser";
        const string password = "testpassword";

        // Act
        var result = ConnectToolParameters.Create(siteRootAddress, userName, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(siteRootAddress, result.SiteRootAddress);
        Assert.Equal(userName, result.UserName);
        Assert.Equal(password, result.Password);
    }

    [Fact]
    public void Create_WithNullSiteRootAddress_ReturnsNull()
    {
        // Arrange
        const string userName = "testuser";
        const string password = "testpassword";

        // Act
        var result = ConnectToolParameters.Create(null, userName, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_WithNullUserName_ReturnsNull()
    {
        // Arrange
        const string siteRootAddress = "https://example.com";
        const string password = "testpassword";

        // Act
        var result = ConnectToolParameters.Create(siteRootAddress, null, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_WithNullPassword_ReturnsNull()
    {
        // Arrange
        const string siteRootAddress = "https://example.com";
        const string userName = "testuser";

        // Act
        var result = ConnectToolParameters.Create(siteRootAddress, userName, null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_WithEmptySiteRootAddress_ReturnsNull()
    {
        // Arrange
        const string userName = "testuser";
        const string password = "testpassword";

        // Act
        var result = ConnectToolParameters.Create(string.Empty, userName, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_WithEmptyUserName_ReturnsNull()
    {
        // Arrange
        const string siteRootAddress = "https://example.com";
        const string password = "testpassword";

        // Act
        var result = ConnectToolParameters.Create(siteRootAddress, string.Empty, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_WithEmptyPassword_ReturnsNull()
    {
        // Arrange
        const string siteRootAddress = "https://example.com";
        const string userName = "testuser";

        // Act
        var result = ConnectToolParameters.Create(siteRootAddress, userName, string.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_WithWhitespaceSiteRootAddress_ReturnsNull()
    {
        // Arrange
        const string userName = "testuser";
        const string password = "testpassword";

        // Act
        var result = ConnectToolParameters.Create("   ", userName, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_WithWhitespaceUserName_ReturnsNull()
    {
        // Arrange
        const string siteRootAddress = "https://example.com";
        const string password = "testpassword";

        // Act
        var result = ConnectToolParameters.Create(siteRootAddress, "   ", password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_WithWhitespacePassword_ReturnsNull()
    {
        // Arrange
        const string siteRootAddress = "https://example.com";
        const string userName = "testuser";

        // Act
        var result = ConnectToolParameters.Create(siteRootAddress, userName, "   ");

        // Assert
        Assert.Null(result);
    }
}
