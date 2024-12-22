using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ConnectTools;

public /*open*/ class CTools
{
    protected readonly string HostName;
    protected readonly ILogger Logger;
    protected readonly ConnectToolParameters Parameters;
    protected readonly int Port;
    protected readonly string StartPath;
    protected readonly bool UseConsole;


    protected CTools(ConnectToolParameters parameters, ILogger logger, bool useConsole = false)
    {
        Parameters = parameters;
        Logger = logger;
        UseConsole = useConsole;
        if (!Uri.TryCreate(Parameters.SiteRootAddress, UriKind.Absolute, out var uri))
            throw new Exception($"Invalid Site Root Address {Parameters.SiteRootAddress}");
        HostName = uri.Host;
        StartPath = uri.AbsolutePath;
        Port = uri.Port;
    }

    public virtual char DirectorySeparatorChar => '/';


    public virtual bool DirectoryExists(string? afterRootPath, string dirName)
    {
        return false;
    }

    public virtual bool CreateDirectory(string? afterRootPath, string dirName)
    {
        return false;
    }

    public virtual List<MyFileInfo> GetFilesWithInfo(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return [];
    }

    public virtual List<string> GetFiles(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return [];
    }

    public virtual List<string> GetSubdirectories(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return [];
    }

    public virtual bool Delete(string? afterRootPath, string fileName)
    {
        return false;
    }

    //, string serverRootPaths
    public virtual bool DeleteDirectory(string? afterRootPath, string dirName)
    {
        return false;
    }

    //public virtual bool DeleteDirectory(string? afterRootPath)
    //{
    //    return false;
    //}


    public virtual bool UploadFile(string pathToFile, string? afterRootPath = null, string? serverSideFileName = null,
        bool allBytesAtOnce = false)
    {
        return false;
    }

    public virtual bool DownloadFile(string? afterRootPath, string fileName, string folderToDownload,
        string? localFileName = null)
    {
        return false;
    }

    public virtual string? GetTextFileContent(string? afterRootPath, string fileName)
    {
        return null;
    }

    public virtual bool UploadFileToDirectory(string pathToFile, string afterRootPath)
    {
        return false;
    }

    public virtual bool UploadContentToTextFile(string content, string? afterRootPath, string serverSideFileName)
    {
        return false;
    }

    public virtual Task<bool> UploadContentToTextFileAsync(string content, string? afterRootPath,
        string serverSideFileName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public virtual bool CreateServerSideUser(string companyName)
    {
        return false;
    }

    public virtual bool CheckConnection()
    {
        return false;
    }

    public virtual bool Rename(string? afterRootPath, string fromFileName, string toFileName)
    {
        return false;
    }


    public virtual bool FileExists(string? afterRootPath, string fileName)
    {
        return false;
    }
}