using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConnectTools;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using SystemToolsShared;

namespace SSHTools;

public sealed class SshTools : CTools
{
    private readonly string? _createUserRemoteCommand;

    public SshTools(string? createUserRemoteCommand, ConnectToolParameters parameters, ILogger logger) : base(
        parameters, logger)
    {
        _createUserRemoteCommand = createUserRemoteCommand;
    }

    public SshTools(ConnectToolParameters parameters, ILogger logger) : base(parameters, logger)
    {
        _createUserRemoteCommand = null;
    }


    public override bool FileExists(string? afterRootPath, string fileName)
    {
        if (!Uri.TryCreate(fileName, UriKind.Absolute, out var uri))
            return false;
        using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
        try
        {
            sftp.Connect();
            SftpFileAttributes? attrs = null;
            if (sftp.Exists(uri.LocalPath))
                attrs = sftp.GetAttributes(uri.LocalPath);
            if (attrs != null)
                return attrs.IsRegularFile;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "File Exists error");
        }
        finally
        {
            sftp.Disconnect();
        }

        return false;
    }


    public override bool DirectoryExists(string? afterRootPath, string dirName)
    {
        if (!Uri.TryCreate(dirName, UriKind.Absolute, out var uri))
            return false;
        using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
        try
        {
            sftp.Connect();
            SftpFileAttributes? attrs = null;
            if (sftp.Exists(uri.LocalPath))
                attrs = sftp.GetAttributes(uri.LocalPath);
            if (attrs != null)
                return attrs.IsDirectory;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Directory Exists error");
        }
        finally
        {
            sftp.Disconnect();
        }

        return false;
    }

    public override bool CheckConnection()
    {
        using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
        try
        {
            sftp.Connect();
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Check Connection error");
        }
        finally
        {
            sftp.Disconnect();
        }

        return false;
    }


    public override bool CreateDirectory(string? afterRootPath, string dirName)
    {
        if (!Uri.TryCreate(dirName, UriKind.Absolute, out var uri))
            return false;
        using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
        try
        {
            sftp.Connect();

            var path = uri.LocalPath;

            var current = "";

            if (path[0] == '/') path = path.Substring(1);

            while (!string.IsNullOrWhiteSpace(path))
            {
                var p = path.IndexOf('/');
                current += '/';
                if (p >= 0)
                {
                    current += path.Substring(0, p);
                    path = path.Substring(p + 1);
                }
                else
                {
                    current += path;
                    path = "";
                }

                if (!sftp.Exists(current))
                {
                    sftp.CreateDirectory(current);
                }
                else
                {
                    var attrs = sftp.GetAttributes(current);
                    if (!attrs.IsDirectory) throw new Exception("not directory");
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Create Directory error");
        }
        finally
        {
            sftp.Disconnect();
        }

        return false;
    }


    public override List<MyFileInfo> GetFilesWithInfo(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return GetDirectoryDetails(afterRootPath, false, fullNames, searchPattern);
    }

    public override List<string> GetFiles(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return GetDirectoryDetails(afterRootPath, false, fullNames, searchPattern).Select(s => s.FileName).ToList();
    }


    public override List<string> GetSubdirectories(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return GetDirectoryDetails(afterRootPath, true, fullNames, searchPattern).Select(s => s.FileName).ToList();
    }

    //private long GetFileLength(string dir, string fileName)
    //{
    //    if (!Uri.TryCreate(dir, UriKind.Absolute, out var uri))
    //        return 0;
    //    using SftpClient sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
    //    sftp.Connect();

    //    IEnumerable<SftpFile> files = sftp.ListDirectory(uri.LocalPath);

    //    SftpFile? file = files.SingleOrDefault(c => !(c.IsDirectory ^ false) && c.Name == fileName);

    //    sftp.Disconnect();

    //    return file?.Length ?? 0;
    //}

    private List<MyFileInfo> GetDirectoryDetails(string? afterRootPath, bool dirs, bool fullNames, string? sFileMask)
    {
        var result = new List<MyFileInfo>();

        if (!Uri.TryCreate(afterRootPath, UriKind.Absolute, out var uri))
            return result;

        using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
        sftp.Connect();

        IEnumerable<SftpFile> files = sftp.ListDirectory(uri.LocalPath);

        result.AddRange(files
            .Where(c => !(c.IsDirectory ^ dirs) && (sFileMask is null || c.Name.FitsMask(sFileMask)) &&
                        c.Name != "." && c.Name != "..").Select(file =>
                new MyFileInfo(fullNames ? file.FullName : file.Name, file.Length)));

        sftp.Disconnect();

        return result;
    }

    public override bool DownloadFile(string? afterRootPath, string fileName, string folderToDownload,
        string? localFileName = null)
    {
        try
        {
            var fileFullName = Path.Combine(folderToDownload, localFileName ?? fileName);

            if (!Uri.TryCreate(afterRootPath, UriKind.Absolute, out var uri))
            {
                Logger.LogError("Invalid URI: " + afterRootPath);
                return false;
            }

            using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
            sftp.Connect();

            using (var fs = File.Create(fileFullName))
            {
                sftp.DownloadFile(uri.LocalPath + "/" + fileName, fs, DownloadCallback);
                fs.Close();
            }

            sftp.Disconnect();
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Download File error");
            return false;
        }
    }


    private void DownloadCallback(ulong position)
    {
        //if (_bp != null)
        //  _bp.SubCounted = (long)position;
    }

    public override bool UploadFile(string pathToFile, string? afterRootPath = null,
        string? serverSideFileName = null, bool allBytesAtOnce = false)
    {
        try
        {
            if (!Uri.TryCreate(afterRootPath ?? ".", UriKind.Absolute, out var uri))
            {
                Logger.LogError("Invalid URI: " + afterRootPath);
                return false;
            }

            using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);

            sftp.Connect();

            using (var fs = File.OpenRead(pathToFile))
            {
                var destinationFileName = serverSideFileName ?? Path.GetFileName(pathToFile);
                if (allBytesAtOnce)
                    sftp.UploadFile(fs, uri.LocalPath + "/" + destinationFileName);
                else
                    sftp.UploadFile(fs, uri.LocalPath + "/" + destinationFileName, UploadCallback);
                fs.Close();
            }

            sftp.Disconnect();

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Upload File error");
            return false;
        }
    }


    private void UploadCallback(ulong position)
    {
        //if (_bp != null)
        //  _bp.SubCounted = (long)position;
    }

    public override bool UploadFileToDirectory(string pathToFile, string afterRootPath)
    {
        try
        {
            if (!Uri.TryCreate(afterRootPath, UriKind.Absolute, out var uri))
            {
                Logger.LogWarning("Invalid URI: " + afterRootPath);
                return false;
            }

            using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);

            sftp.Connect();

            using (var fs = File.OpenRead(pathToFile))
            {
                sftp.UploadFile(fs, uri.LocalPath + "/" + Path.GetFileName(pathToFile));
            }

            sftp.Disconnect();

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error on UploadFileToDirectory");
            return false;
        }
    }

    public override bool CreateServerSideUser(string companyName)
    {
        if (_createUserRemoteCommand == null)
            return false;

        using var sshClient = new SshClient(HostName, Parameters.UserName, Parameters.Password);

        try
        {
            sshClient.Connect();

            var sshCom = sshClient.RunCommand(string.Format(_createUserRemoteCommand, companyName));

            Console.WriteLine(sshCom.Result);

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Create Server Side User error");
        }
        finally
        {
            sshClient.Disconnect();
        }

        return false;
    }


    public override bool Delete(string? afterRootPath, string fileName)
    {
        using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
        try
        {
            sftp.Connect();
            sftp.DeleteFile(afterRootPath + "/" + fileName);
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Delete file error");
        }
        finally
        {
            sftp.Disconnect();
        }

        return false;
    }


    public override bool DeleteDirectory(string? afterRootPath, string dirName)
    {
        using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
        try
        {
            sftp.Connect();
            sftp.DeleteDirectory(dirName);
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Delete file error");
        }
        finally
        {
            sftp.Disconnect();
        }

        return false;
    }


    public override bool Rename(string? afterRootPath, string fromFileName, string toFileName)
    {
        using var sftp = new SftpClient(HostName, Parameters.UserName, Parameters.Password);
        try
        {
            sftp.Connect();
            sftp.RenameFile(afterRootPath + "/" + fromFileName, afterRootPath + "/" + toFileName);
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Rename file error");
        }
        finally
        {
            sftp.Disconnect();
        }

        return false;
    }
}