﻿using System;
using ConnectTools;
using FTPTools;
using Microsoft.Extensions.Logging;

//using SSHTools;

namespace CToolsFactory;

public static class ConnectToolsFactory
{
    public static CTools? CreateConnectToolsByAddress(ConnectToolParameters parameters, ILogger logger,
        bool useConsole = false)
    {
        if (!Uri.TryCreate(parameters.SiteRootAddress, UriKind.Absolute, out var uri))
            return null;

        return uri.Scheme.ToLower() switch
        {
            "ftp" => new FtpTools(parameters, logger, useConsole),
            //"ssh" => new SshTools(parameters, logger),
            _ => null
        };
    }
}