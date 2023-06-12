﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConnectTools;
using FluentFTP;
using Microsoft.Extensions.Logging;
using SystemToolsShared;

namespace FTPTools;

public sealed class FtpTools : CTools
{
    public FtpTools(ConnectToolParameters parameters, ILogger logger, bool useConsole = false) : base(parameters,
        logger, useConsole)
    {
    }


    private FtpClient CreateFtpClient()
    {
        return new FtpClient(HostName, Parameters.UserName, Parameters.Password, Port);
    }

    public override bool CheckConnection()
    {
        try
        {
            var ftpClient = CreateFtpClient();
            ftpClient.Config.ConnectTimeout *= 256;
            ftpClient.Connect();
            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Ftp client Check Connection Failed");
        }

        return false;
    }

    public override bool DownloadFile(string? afterRootPath, string fileName, string folderToDownload,
        string? localFileName = null)
    {
        try
        {
            var source = GetRemotePath(afterRootPath, fileName);
            var fileFullName = Path.Combine(folderToDownload, localFileName ?? fileName);

            if (UseConsole)
                Console.WriteLine($"Download file from {source} to {fileFullName}");

            using var ftp = CreateFtpClient();

            ftp.Connect();

            /* For Progress use
             * Action<FtpProgress> progress = delegate(FtpProgress p){
                if (p.Progress == 1) {
                    // all done!
                }
                else {
                    // percent done = (p.Progress * 100)
                }
            };
            // download a file with progress tracking
            ftp.DownloadFile(@"D:\Github\FluentFTP\README.md", "/public_html/temp/README.md", FtpLocalExists.Overwrite, FtpVerify.None, progress);
             */

            ftp.DownloadFile(fileFullName, source);

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Download File error");
            return false;
        }
    }

    public override string? GetTextFileContent(string? afterRootPath, string fileName)
    {
        try
        {
            var remoteFilePath = GetRemotePath(afterRootPath, fileName);

            using var ftp = CreateFtpClient();

            ftp.Connect();

            try
            {
                // stream.Position is incremented accordingly to the writes you perform
                //int bufSize = (int)stream.Length;
                //byte[] buffer = new byte[bufSize];
                if (ftp.DownloadBytes(out var buffer, remoteFilePath))

                    //stream.Read(buffer, 0, bufSize);
                    return Encoding.UTF8.GetString(buffer);
                Logger.LogError("File did not Downloaded");
                return null;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Download File content error");
                return null;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Download File content error");
            return null;
        }
    }

    public override bool UploadContentToTextFile(string content, string? afterRootPath,
        string serverSideFileName)
    {
        try
        {
            var remoteFilePath = GetRemotePath(afterRootPath, serverSideFileName);


            using var ftp = CreateFtpClient();
            ftp.Connect();

            // open an write-only stream to the file
            //using var stream = ftp.OpenWrite(remoteFilePath);
            try
            {
                // stream.Position is incremented accordingly to the writes you perform
                // convert string to stream
                var byteArray = Encoding.UTF8.GetBytes(content);
                //stream.Write(byteArray, 0, byteArray.Length);

                if (ftp.UploadBytes(byteArray, remoteFilePath) == FtpStatus.Success)
                    return true;
                Logger.LogError("File did not Uploaded");
                return false;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "error on UploadFileToDirectory");
                return false;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "error on UploadFileToDirectory");
            return false;
        }
    }

    private string GetRemotePath(string? afterRootPath, string fileName)
    {
        return Path.Combine(GetRemotePath(afterRootPath), fileName)
            .Replace(Path.DirectorySeparatorChar, DirectorySeparatorChar);
    }

    private string GetRemotePath(string? afterRootPath)
    {
        return afterRootPath == null
            ? StartPath
            : Path.Combine(StartPath, afterRootPath)
                .Replace(Path.DirectorySeparatorChar, DirectorySeparatorChar);
    }

    public override bool UploadFile(string pathToFile, string? afterRootPath = null,
        string? serverSideFileName = null, bool allBytesAtOnce = false)
    {
        var remoteFilePath = GetRemotePath(afterRootPath, serverSideFileName ?? Path.GetFileName(pathToFile));

        try
        {
            //if (UseConsole)
            //    Console.WriteLine($"FTPTools UploadFile destination = {remoteFilePath}");
            Logger.LogInformation($"FTPTools UploadFile destination = {remoteFilePath}");

            using var ftp = CreateFtpClient();
            ftp.Connect();

            // upload a file to an existing FTP directory
            ftp.UploadFile(pathToFile, remoteFilePath);

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "UploadFile error");
            return false;
        }
    }

    public override bool UploadFileToDirectory(string pathToFile, string afterRootPath)
    {
        return UploadFile(pathToFile, afterRootPath);
    }

    public override bool FileExists(string? afterRootPath, string fileName)
    {
        var remoteFilePath = GetRemotePath(afterRootPath, fileName);

        using var conn = CreateFtpClient();
        conn.Connect();

        // The last parameter forces FluentFTP to use LIST -a 
        // for getting a list of objects in the parent directory.
        return conn.FileExists(remoteFilePath);
    }

    public override bool DirectoryExists(string? afterRootPath, string dirName)
    {
        var remoteFolderPath = GetRemotePath(afterRootPath, dirName);

        using var conn = CreateFtpClient();
        conn.Connect();

        // The last parameter forces FluentFTP to use LIST -a 
        // for getting a list of objects in the parent directory.
        return conn.DirectoryExists(remoteFolderPath);
    }

    public override List<MyFileInfo> GetFilesWithInfo(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return GetDirectoryDetails(afterRootPath, false, true, fullNames, logError, searchPattern);
    }

    public override List<string> GetFiles(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return GetDirectoryDetails(afterRootPath, false, true, fullNames, logError, searchPattern)
            .Select(s => s.FileName).ToList();
    }

    public override List<string> GetSubdirectories(string? afterRootPath, string? searchPattern, bool logError,
        bool fullNames = false)
    {
        return GetDirectoryDetails(afterRootPath, true, false, fullNames, logError, searchPattern)
            .Select(s => s.FileName).ToList();
    }

    private List<MyFileInfo> GetDirectoryDetails(string? afterRootPath, bool dirs, bool files, bool fullNames,
        bool logError, string? mask)
    {
        var result = new List<MyFileInfo>();

        try
        {
            var remotePath = GetRemotePath(afterRootPath);

            using var conn = CreateFtpClient();
            conn.Connect();

            // get listing of the files & folders in a specific folder
            result.AddRange(conn.GetListing(remotePath)
                .Where(item =>
                    ((dirs && item.Type == FtpObjectType.Directory) ||
                     (files && item.Type == FtpObjectType.File)) &&
                    (mask is null || item.Name.FitsMask(mask)))
                .Select(item => new MyFileInfo(fullNames ? item.FullName : item.Name,
                    conn.GetFileSize(item.FullName))));
        }
        catch (Exception e)
        {
            if (logError)
                Logger.LogError(e, "GetDirectoryDetails error");
        }

        return result;
    }

    public override bool Delete(string? afterRootPath, string fileName)
    {
        try
        {
            var remoteFilePath = GetRemotePath(afterRootPath, fileName);

            if (UseConsole)
                Console.WriteLine($"Delete file {remoteFilePath}");

            using var ftp = CreateFtpClient();

            ftp.Connect();

            ftp.DeleteFile(remoteFilePath);

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Delete File error");
            return false;
        }
    }

    public override bool DeleteDirectory(string? afterRootPath, string dirName)
    {
        try
        {
            var remoteDirName = GetRemotePath(afterRootPath, dirName);

            using var ftp = CreateFtpClient();

            ftp.Connect();

            ftp.DeleteDirectory(remoteDirName);

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Delete Directory error");
            return false;
        }
    }

    //public override bool DeleteDirectory(string? afterRootPath)
    //{

    //    try
    //    {
    //        string remoteDirPath = GetRemotePath(afterRootPath);

    //        using FtpClient ftp = CreateFtpClient();

    //        ftp.Connect();

    //        ftp.DeleteDirectory(remoteDirPath);

    //        return true;
    //    }
    //    catch (Exception e)
    //    {
    //        Logger.LogError(e, "Delete Directory error");
    //        return false;
    //    }
    //}

    public override bool Rename(string? afterRootPath, string fromFileName, string toFileName)
    {
        try
        {
            var remoteFromFileName = GetRemotePath(afterRootPath, fromFileName);

            var remoteToFileName = GetRemotePath(afterRootPath, toFileName);

            using var ftp = CreateFtpClient();

            ftp.Connect();

            ftp.Rename(remoteFromFileName, remoteToFileName);

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Rename File error");
            return false;
        }
    }

    public override bool CreateDirectory(string? afterRootPath, string dirName)
    {
        try
        {
            var remoteFolderName = GetRemotePath(afterRootPath, dirName);

            using var ftp = CreateFtpClient();

            ftp.Connect();

            ftp.CreateDirectory(remoteFolderName);

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Create Directory error");
            return false;
        }
    }
}