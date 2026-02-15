using Microsoft.Extensions.Logging;
using Moq;

namespace ConnectionTools.ConnectTools.Tests;

public sealed class CToolsTests
{
    private readonly Mock<ILogger> _mockLogger;

    public CToolsTests()
    {
        _mockLogger = new Mock<ILogger>();
    }

    private ConnectToolParameters CreateValidParameters(string siteRootAddress = "https://example.com/path")
    {
        return ConnectToolParameters.Create(siteRootAddress, "testuser", "testpassword")!;
    }

    private TestCTools CreateCTools(ConnectToolParameters? parameters = null, bool useConsole = false)
    {
        parameters ??= CreateValidParameters();
        return new TestCTools(parameters, _mockLogger.Object, useConsole);
    }

    [Fact]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // Arrange
        var parameters = CreateValidParameters("https://example.com:8080/mypath");

        // Act
        var cTools = CreateCTools(parameters);

        // Assert
        Assert.Equal("example.com", cTools.GetHostName());
        Assert.Equal("/mypath", cTools.GetStartPath());
        Assert.Equal(8080, cTools.GetPort());
        Assert.Equal(parameters, cTools.GetParameters());
        Assert.False(cTools.GetUseConsole());
    }

    [Fact]
    public void Constructor_WithInvalidSiteRootAddress_ThrowsException()
    {
        // Arrange
        var parameters = ConnectToolParameters.Create("not a valid url", "user", "pass")!;

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => CreateCTools(parameters));
        Assert.Contains("Invalid Site Root Address", exception.Message);
        Assert.Contains("not a valid url", exception.Message);
    }

    [Fact]
    public void Constructor_WithHttpUrl_ParsesCorrectly()
    {
        // Arrange
        var parameters = CreateValidParameters("http://test.com");

        // Act
        var cTools = CreateCTools(parameters);

        // Assert
        Assert.Equal("test.com", cTools.GetHostName());
        Assert.Equal("/", cTools.GetStartPath());
        Assert.Equal(80, cTools.GetPort());
    }

    [Fact]
    public void Constructor_WithHttpsUrl_ParsesCorrectly()
    {
        // Arrange
        var parameters = CreateValidParameters("https://secure.com");

        // Act
        var cTools = CreateCTools(parameters);

        // Assert
        Assert.Equal("secure.com", cTools.GetHostName());
        Assert.Equal("/", cTools.GetStartPath());
        Assert.Equal(443, cTools.GetPort());
    }

    [Fact]
    public void Constructor_WithUseConsoleTrue_SetsProperty()
    {
        // Arrange
        var parameters = CreateValidParameters();

        // Act
        var cTools = CreateCTools(parameters, true);

        // Assert
        Assert.True(cTools.GetUseConsole());
    }

    [Fact]
    public void DirectorySeparatorChar_ReturnsForwardSlash()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var separator = cTools.DirectorySeparatorChar;

        // Assert
        Assert.Equal('/', separator);
    }

    [Fact]
    public void DirectoryExists_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.DirectoryExists("some/path", "dirname");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DirectoryExists_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.DirectoryExists(null, "dirname");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreateDirectory_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.CreateDirectory("some/path", "newdir");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreateDirectory_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.CreateDirectory(null, "newdir");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetFilesWithInfo_WithAnyParameters_ReturnsEmptyList()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.GetFilesWithInfo("path", "*.txt", true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetFilesWithInfo_WithNullParameters_ReturnsEmptyList()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.GetFilesWithInfo(null, null, false, true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetFiles_WithAnyParameters_ReturnsEmptyList()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.GetFiles("path", "*.txt", true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetFiles_WithNullParameters_ReturnsEmptyList()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.GetFiles(null, null, false, true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetSubdirectories_WithAnyParameters_ReturnsEmptyList()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.GetSubdirectories("path", "*", true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetSubdirectories_WithNullParameters_ReturnsEmptyList()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.GetSubdirectories(null, null, false, true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Delete_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.Delete("path", "file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Delete_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.Delete(null, "file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DeleteDirectory_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.DeleteDirectory("path", "dirname");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DeleteDirectory_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.DeleteDirectory(null, "dirname");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UploadFile_WithRequiredParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.UploadFile("C:\\temp\\file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UploadFile_WithAllParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.UploadFile("C:\\temp\\file.txt", "path", "newname.txt", true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UploadFile_WithNullOptionalParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.UploadFile("C:\\temp\\file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DownloadFile_WithRequiredParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.DownloadFile("path", "file.txt", "C:\\downloads");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DownloadFile_WithAllParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.DownloadFile("path", "file.txt", "C:\\downloads", "renamed.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DownloadFile_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.DownloadFile(null, "file.txt", "C:\\downloads");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetTextFileContent_WithAnyParameters_ReturnsNull()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.GetTextFileContent("path", "file.txt");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetTextFileContent_WithNullPath_ReturnsNull()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.GetTextFileContent(null, "file.txt");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UploadFileToDirectory_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.UploadFileToDirectory("C:\\temp\\file.txt", "target/path");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UploadContentToTextFile_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.UploadContentToTextFile("content", "path", "file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UploadContentToTextFile_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.UploadContentToTextFile("content", null, "file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UploadContentToTextFileAsync_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = await cTools.UploadContentToTextFileAsync("content", "path", "file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UploadContentToTextFileAsync_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = await cTools.UploadContentToTextFileAsync("content", null, "file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UploadContentToTextFileAsync_WithCancellationToken_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();
        using var cts = new CancellationTokenSource();

        // Act
        var result = await cTools.UploadContentToTextFileAsync("content", "path", "file.txt", cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreateServerSideUser_WithAnyParameter_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.CreateServerSideUser("CompanyName");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CheckConnection_ReturnsDefault_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.CheckConnection();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Rename_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.Rename("path", "oldname.txt", "newname.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Rename_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.Rename(null, "oldname.txt", "newname.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FileExists_WithAnyParameters_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.FileExists("path", "file.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FileExists_WithNullPath_ReturnsFalse()
    {
        // Arrange
        var cTools = CreateCTools();

        // Act
        var result = cTools.FileExists(null, "file.txt");

        // Assert
        Assert.False(result);
    }

    // Test helper class to expose protected members for testing
    private sealed class TestCTools : CTools
    {
        public TestCTools(ConnectToolParameters parameters, ILogger logger, bool useConsole = false) : base(parameters,
            logger, useConsole)
        {
        }

        public string GetHostName() => HostName;
        public string GetStartPath() => StartPath;
        public int GetPort() => Port;
        public ConnectToolParameters GetParameters() => Parameters;
        public bool GetUseConsole() => UseConsole;
    }
}
